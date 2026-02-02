using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;


namespace DevOpsCopilot.Cli
{

    public sealed class DevOpsPlugin
    {
        private readonly FileBuildLogReader _logReader;

        public DevOpsPlugin(FileBuildLogReader logReader)
        {
            _logReader = logReader;
        }

        [KernelFunction("read_build_log")]
        public string ReadBuildLog(string path)
        {
            return _logReader.Read(path);
        }

        [KernelFunction("deploy_stage")]
        public string DeployStage()
        {
            return "Staging deployment started and completed successfully.";
        }

        [KernelFunction("deploy_prod")]
        public string DeployProd()
        {
            return "Production deployment started and completed successfully.";
        }

        [KernelFunction("create_branch")]
        public string CreateBranch(string branchName, string baseBranch)
        {
            return $"Created branch '{branchName}' from '{baseBranch}'.";
        }
    }

}
