using Estacionamento.Infrastructure;
using System;
using System.IO;

namespace Estacionamento.Services
{
    public sealed class DatabaseBackupService
    {
        private const string DatabaseFileName = "estacionamento.db";

        public string DatabasePath => AppPaths.GetDataFilePath(DatabaseFileName);

        public string CriarBackup(string destino)
        {
            if (string.IsNullOrWhiteSpace(destino))
            {
                throw new ArgumentException("Informe o caminho de destino do backup.", nameof(destino));
            }

            var directory = Path.GetDirectoryName(destino);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(DatabasePath, destino, overwrite: true);
            return destino;
        }

        public static string CriarNomePadrao()
        {
            return $"parksystem-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db";
        }
    }
}
