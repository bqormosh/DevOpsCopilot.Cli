using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;


namespace DevOpsCopilot.Cli
{


    public sealed class ApprovalFilter : IFunctionInvocationFilter
    {
        private readonly ILogger<ApprovalFilter> _logger;

        public ApprovalFilter(ILogger<ApprovalFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            var isProdDeploy = string.Equals(context.Function.PluginName, "devops", StringComparison.OrdinalIgnoreCase)
                && string.Equals(context.Function.Name, "deploy_prod", StringComparison.OrdinalIgnoreCase);

            if (isProdDeploy)
            {
                Console.WriteLine("System: Approve production deployment? Type Y to approve.");
                Console.Write("User: ");
                var input = Console.ReadLine() ?? "";
                var approved = string.Equals(input.Trim(), "Y", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("Production deployment approval: {Approved}", approved);

                if (!approved)
                {
                    context.Result = new FunctionResult(context.Result, "Production deployment was not approved.");
                    return;
                }
            }

            await next(context);
        }
    }

}
