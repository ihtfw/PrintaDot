using System.Drawing.Printing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.Drawing;

namespace PrintaDot.Windows
{
    public class WindowsPrintingService
    {
        private string _printerName;

        public WindowsPrintingService(string printerName = "Microsoft Print to PDF")
        {
            _printerName = printerName;
        }

        public void PrintImage(SixLabors.ImageSharp.Image image)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"barcode_{Guid.NewGuid()}.png");

            try
            {
                image.Save(tempFile, new PngEncoder());

                using var bitmap = new Bitmap(tempFile);
                using var printDocument = new PrintDocument();

                printDocument.PrinterSettings.PrinterName = _printerName;

                float dpiX = bitmap.HorizontalResolution;
                float dpiY = bitmap.VerticalResolution;

                int paperWidth = (int)(bitmap.Width / dpiX * 100.0f);
                int paperHeight = (int)(bitmap.Height / dpiY * 100.0f);

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
                    e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                    e.HasMorePages = false;
                };

                printDocument.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка печати");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
