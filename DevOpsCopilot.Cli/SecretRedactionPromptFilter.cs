using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;

namespace DevOpsCopilot.Cli
{


    public sealed class SecretRedactionPromptFilter : IPromptRenderFilter
    {
        private readonly ILogger<SecretRedactionPromptFilter> _logger;

        public SecretRedactionPromptFilter(ILogger<SecretRedactionPromptFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await next(context);

            if (string.IsNullOrWhiteSpace(context.RenderedPrompt))
            {
                return;
            }

            var before = context.RenderedPrompt;

            var after = Regex.Replace(before, @"(?i)(api[-_ ]?key\s*[:=]\s*)([^\s""']+)", "$1***REDACTED***");
            after = Regex.Replace(after, @"(?i)(authorization\s*:\s*bearer\s+)([^\s]+)", "$1***REDACTED***");
            after = Regex.Replace(after, @"(?i)(connectionstring\s*[:=]\s*)(.+)", "$1***REDACTED***");

            if (!string.Equals(before, after, StringComparison.Ordinal))
            {
                _logger.LogInformation("Prompt redaction applied for {Function}", context.Function.Name);
            }

            context.RenderedPrompt = after;
        }
    }

}
