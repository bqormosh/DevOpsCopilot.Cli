using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsCopilot.Cli
{
    public sealed class DevOpsCopilotOptions
    {
        public string BuildLogPath { get; set; } = "Files/build.log";
        public string SessionsPath { get; set; } = "Sessions";
    }
}
