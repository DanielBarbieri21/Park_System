using Estacionamento.Abstractions;
using System;
using System.IO;

namespace Estacionamento.Infrastructure
{
    public sealed class FileLogService : ILogService
    {
        private readonly string _logPath;
        private readonly object _sync = new();

        public FileLogService(string? baseDirectory = null)
        {
            var root = baseDirectory ?? AppContext.BaseDirectory;
            var logsDir = Path.Combine(root, "logs");
            Directory.CreateDirectory(logsDir);
            _logPath = Path.Combine(logsDir, $"app-{DateTime.UtcNow:yyyyMMdd}.log");
        }

        public void Info(string message) => Write("INFO", message, null);

        public void Error(string message, Exception? exception = null) => Write("ERROR", message, exception);

        private void Write(string level, string message, Exception? exception)
        {
            var line = $"{DateTime.UtcNow:O} [{level}] {message}";
            if (exception != null)
            {
                line += $" | {exception.GetType().Name}: {exception.Message}";
            }

            lock (_sync)
            {
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
        }
    }
}
