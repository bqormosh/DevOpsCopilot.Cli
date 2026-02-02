using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace DevOpsCopilot.Cli
{


    public sealed class ConversationStore
    {
        private readonly string _sessionsPath;

        public ConversationStore(string sessionsPath)
        {
            _sessionsPath = sessionsPath;
        }

        public async Task SaveAsync(ChatHistory history)
        {
            Directory.CreateDirectory(_sessionsPath);

            var items = history.Select(m => new
            {
                Role = m.Role.Label,
                Content = m.Content
            }).ToArray();

            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"session-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
            var fullPath = Path.Combine(_sessionsPath, fileName);

            await File.WriteAllTextAsync(fullPath, json);
        }
    }

}
