using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace DevOpsCopilot.Cli
{

    public sealed class FileBuildLogReader
    {
        private readonly string _baseDirectory;

        public FileBuildLogReader(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public string Read(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Path is required.", nameof(relativePath));
            }

            var fullPath = Path.GetFullPath(Path.Combine(_baseDirectory, relativePath));
            var allowedRoot = Path.GetFullPath(Path.Combine(_baseDirectory, "Files"));

            if (!fullPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityException("Access denied.");
            }

            return File.ReadAllText(fullPath);
        }
    }

}
