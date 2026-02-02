using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsCopilot.Cli
{
    public sealed class BuildLogDecision
    {
        public bool HasErrors { get; set; }
        public bool SafeToDeployStage { get; set; }
        public string Summary { get; set; } = "";
        public string[] TopFindings { get; set; } = [];
    }

}
