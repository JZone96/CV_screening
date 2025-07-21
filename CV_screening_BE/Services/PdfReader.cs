using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace CVToolApi.Services
{
    public static class PdfReader
    {
        public static void ExtractPdfText(string pdfPath) {
             ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");

            using (PdfDocument document = PdfDocument.Open(pdfPath))
            {
                foreach (Page page in document.GetPages())
                {

                    string pageText = page.Text;
                    foreach (Word word in page.GetWords())
                    {
                        Console.WriteLine(word.Text);
                    }
                }
            }
        }

    }

}
