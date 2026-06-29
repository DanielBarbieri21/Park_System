using System;
using System.IO;

namespace Estacionamento.Infrastructure
{
    public static class AppPaths
    {
        public static string DataDirectory
        {
            get
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ParkSystem",
                    "Estacionamento");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public static string LogsDirectory
        {
            get
            {
                var directory = Path.Combine(DataDirectory, "logs");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public static string GetDataFilePath(string fileName)
        {
            var targetPath = Path.Combine(DataDirectory, fileName);
            if (File.Exists(targetPath))
            {
                return targetPath;
            }

            foreach (var legacyPath in GetLegacyCandidatePaths(fileName))
            {
                if (!File.Exists(legacyPath))
                {
                    continue;
                }

                File.Copy(legacyPath, targetPath, overwrite: false);
                break;
            }

            return targetPath;
        }

        private static string[] GetLegacyCandidatePaths(string fileName)
        {
            return new[]
            {
                Path.Combine(AppContext.BaseDirectory, fileName),
                Path.Combine(Environment.CurrentDirectory, fileName)
            };
        }
    }
}
