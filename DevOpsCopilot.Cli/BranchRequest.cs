using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsCopilot.Cli
{
    public sealed class BranchRequest
    {
        public bool Missing { get; set; }
        public string BranchName { get; set; } = "";
        public string BaseBranch { get; set; } = "";
        public string Question { get; set; } = "";
    }
}
