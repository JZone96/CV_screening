using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace CVToolApi.Services
{

    public static class ZipExtractor
    {
        public static List<string> ExtractPdfFiles(string zipPath, string extractPath)
        {

         ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");

            extractPath = Path.GetFullPath(extractPath);
            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                extractPath += Path.DirectorySeparatorChar;

            ZipArchive archive = ZipFile.OpenRead(zipPath);

            List<string> listOfPaths = new List<string>();
            using (ZipArchive zip = archive)
            {


                foreach (ZipArchiveEntry entry in archive.Entries)
                {

                    if (entry.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        {

                            entry.ExtractToFile(destinationPath, overwrite: true);
                            listOfPaths.Add(destinationPath);
                            logger.LogInformation("Estratto file: in {Destination}", listOfPaths);

                        }

                    }

                }
                return listOfPaths;
            }
        }
    }
}
