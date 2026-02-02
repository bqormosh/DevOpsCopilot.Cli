using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsCopilot.Cli
{
    public sealed class AzureOpenAIOptions
    {
        public string Endpoint { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string DeploymentName { get; set; } = "";
    }

}
