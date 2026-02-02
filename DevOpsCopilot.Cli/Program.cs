using DevOpsCopilot.Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddEnvironmentVariables()
    .Build();

var azure = config.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>() ?? throw new InvalidOperationException("AzureOpenAI config missing.");
var options = config.GetSection("DevOpsCopilot").Get<DevOpsCopilotOptions>() ?? new DevOpsCopilotOptions();

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.Services.AddLogging(b =>
{
    b.ClearProviders();
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Information);
});

kernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: azure.DeploymentName,
    endpoint: azure.Endpoint,
    apiKey: azure.ApiKey
);

kernelBuilder.Services.AddSingleton(new FileBuildLogReader(AppContext.BaseDirectory));
kernelBuilder.Services.AddSingleton<DevOpsPlugin>();
kernelBuilder.Services.AddSingleton<ApprovalFilter>();
kernelBuilder.Services.AddSingleton<SecretRedactionPromptFilter>();
kernelBuilder.Services.AddSingleton(new ConversationStore(Path.Combine(AppContext.BaseDirectory, options.SessionsPath)));

var kernel = kernelBuilder.Build();

kernel.FunctionInvocationFilters.Add(kernel.Services.GetRequiredService<ApprovalFilter>());
kernel.PromptRenderFilters.Add(kernel.Services.GetRequiredService<SecretRedactionPromptFilter>());

kernel.Plugins.AddFromObject(kernel.Services.GetRequiredService<DevOpsPlugin>(), "devops");

var chat = kernel.GetRequiredService<IChatCompletionService>();

var analyzeYaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "AnalyzeBuildLog.yaml"));
var parseBranchYaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "ParseBranchRequest.yaml"));

var handlebarsFactory = new HandlebarsPromptTemplateFactory();
var analyzeBuildLogFn = kernel.CreateFunctionFromPromptYaml(analyzeYaml, handlebarsFactory);
var parseBranchFn = kernel.CreateFunctionFromPromptYaml(parseBranchYaml, handlebarsFactory);

kernel.Plugins.AddFromFunctions("prompts", [analyzeBuildLogFn, parseBranchFn]);

var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage("""
You are DevOps Copilot.

You can call tools:
- devops.read_build_log(path)
- prompts.AnalyzeBuildLog(log)
- prompts.ParseBranchRequest(input)
- devops.create_branch(branchName, baseBranch)
- devops.deploy_stage()
- devops.deploy_prod()

Policies:
- Before staging deployment, always read the build log and analyze it. Deploy to staging only if safeToDeployStage=true.
- For production deployment, you must request approval; the system will enforce approval.
When required info is missing (branch name/base branch), ask a single short question.
""");

var execSettings = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

Console.WriteLine("DevOps Copilot");
Console.WriteLine("Assistant: How may I help you?");
Console.Write("User: ");

while (true)
{
    var input = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    if (string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    chatHistory.AddUserMessage(input);

    var reply = await chat.GetChatMessageContentAsync(
        chatHistory,
        executionSettings: execSettings,
        kernel: kernel
    );

    Console.WriteLine("Assistant: " + reply);
    chatHistory.AddAssistantMessage(reply.ToString());
    Console.Write("User: ");
}

await kernel.Services.GetRequiredService<ConversationStore>().SaveAsync(chatHistory);
