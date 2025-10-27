using System.Drawing.Printing;
using SixLabors.ImageSharp;
using System.Drawing;
using PrintaDot.Shared.Platform;
using SixLabors.ImageSharp.Formats.Bmp;

namespace PrintaDot.Windows
{
    public class WindowsPrintingService : IPlatformPrintingService
    {
        public void Print(string printerName, SixLabors.ImageSharp.Image image)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"barcode_{Guid.NewGuid()}.bmp");

            try
            {
                image.Save(tempFile, new BmpEncoder());

                using var bitmap = new Bitmap(tempFile);
                using var printDocument = new PrintDocument();

                printDocument.PrinterSettings.PrinterName = printerName;

                int scaledWidth = bitmap.Width / 3;
                int scaledHeight = bitmap.Height / 3;

                int paperWidth = (int)(scaledWidth / 96.0f * 100.0f);
                int paperHeight = (int)(scaledHeight / 96.0f * 100.0f);

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
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    e.Graphics.DrawImage(bitmap, 0, 0, scaledWidth, scaledHeight);
                    e.HasMorePages = false;
                };

                printDocument.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка печати: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}