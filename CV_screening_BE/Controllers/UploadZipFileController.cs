using System.Net;
using CVToolApi.Models;
using Microsoft.AspNetCore.Mvc;

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
        var permittedExtensions = new[] { ".zip", ".rar", ".7z" };
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
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        return Ok(new { count = files.Count, size });
    }
}

