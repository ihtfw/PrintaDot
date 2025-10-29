using System.Drawing.Printing;
using SixLabors.ImageSharp;
using System.Drawing;
using PrintaDot.Shared.Platform;
using SixLabors.ImageSharp.Formats.Bmp;
using PrintaDot.Shared.ImageGeneration;

namespace PrintaDot.Windows
{
    public class WindowsPrintingService : IPlatformPrintingService
    {
        public void Print(string printerName, SixLabors.ImageSharp.Image image)
        {
            try
            {
                using var printDocument = new PrintDocument();

                printDocument.PrinterSettings.PrinterName = printerName;

                int paperWidth = (ImageGenerationHelper.FromPixelsToHundredthsInch(image.Width, (float)image.Metadata.HorizontalResolution));
                int paperHeight = (ImageGenerationHelper.FromPixelsToHundredthsInch(image.Height, (float)image.Metadata.VerticalResolution));

                var paperSize = new PaperSize("Custom", paperWidth, paperHeight)
                {
                    RawKind = (int)PaperKind.Custom
                };

                printDocument.DefaultPageSettings.PaperSize = paperSize;
                printDocument.DefaultPageSettings.Landscape = false;
                printDocument.PrinterSettings.DefaultPageSettings.Landscape = false;
                printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                printDocument.PrintPage += (sender, e) =>
                {
                    if (e.Graphics == null)
                    {
                        e.HasMorePages = false;
                        return;
                    }

                    using var ms = new MemoryStream();
                    image.Save(ms, new BmpEncoder());
                    ms.Position = 0;

                    using var bitmap = new Bitmap(ms);
                    e.Graphics?.DrawImage(bitmap, 0, 0);

                    e.HasMorePages = false;
                };

                printDocument.Print();
            }
            catch (Exception ex)
            {
                //TODO - ЕСЛИ ОШИБКА ОТПРАВЛЯТЬ ERRORMESSAGE РАСШИРЕНИЮ
                Console.WriteLine($"Ошибка печати: {ex.Message}");
            }
        }
    }
}