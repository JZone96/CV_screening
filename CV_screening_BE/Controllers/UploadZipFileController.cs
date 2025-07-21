using System.Net;
using CVToolApi.Models;
using Microsoft.AspNetCore.Mvc;
using CVToolApi.Services;
using System.Security.Cryptography.X509Certificates;
namespace CVTool.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadZip(List<IFormFile> files)
    {
        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
        var permittedExtensions = new[] { ".zip" };
        long size = files.Sum(f => f.Length);


        if (size == 0)
        {
            var errorModel = new ErrorResponse
            {
                Message = "Nessun file inserito",
                StatusCode = 400,
            };
            return BadRequest(errorModel);
        }
        logger.LogInformation("Files content.", files);

        var pdfPaths = new List<string>();


        foreach (var formFile in files)
        {
            logger.LogInformation("Hello World! Logging is {Description}.", permittedExtensions.Any(ext => !formFile.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
            if (!permittedExtensions.Any(ext => formFile.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                var errorModel = new ErrorResponse
                {
                    Message = "E' possibile caricare solo file .zip, .rar, .7z",
                    StatusCode = 400,
                };
                return BadRequest(errorModel);
            }
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest();
            }
            if (formFile.Length > 0)
            {
                //salva il file con estensione.zip
                // "cv_"+DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip"
                var filePath = Path.Combine(Path.GetTempPath(), "cv_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip");


                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                // Estrazione pdf nella cartella temp
                var extractPath = Path.Combine(Path.GetTempPath(), "Estratti");
                logger.LogInformation("Hello World! Logging is {Description}.", filePath);

                Directory.CreateDirectory(extractPath);
                pdfPaths = ZipExtractor.ExtractPdfFiles(filePath, extractPath);

            }
        }
        logger.LogInformation("Numero di PDF estratti: {Count}", pdfPaths.Count());

         if (pdfPaths.Count() > 0)
            {
                foreach (var path in pdfPaths)
                {
                    PdfReader.ExtractPdfText(path);
                }
            }

        return Ok(new { count = files.Count, size, estratti = pdfPaths });
    }
}

