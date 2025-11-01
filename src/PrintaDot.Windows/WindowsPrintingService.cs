using System.Drawing.Printing;
using System.Drawing;
using PrintaDot.Shared.Platform;
using SixLabors.ImageSharp.Formats.Bmp;
using PrintaDot.Shared.ImageGeneration;
using PrintaDot.Shared.Printing;

namespace PrintaDot.Windows
{
    public class WindowsPrintingService : IPlatformPrintingService
    {
        public void Print(string printerName, List<SixLabors.ImageSharp.Image> images, PaperSettings paperSettings)
        {
            try
            {
                using var printDocument = SetupPrintDocument(printerName, paperSettings);

                var currentImageIndex = 0;
                var labelsPerPage = paperSettings.LabelsPerRow * paperSettings.LabelsPerColumn;

                var dpi = (float)images.First().Metadata.HorizontalResolution;

                var labelWidthInch = ImageGenerationHelper.FromPixelsToHundredthsInch(images[0].Width, dpi);
                var labelHeightInch = ImageGenerationHelper.FromPixelsToHundredthsInch(images[0].Height, dpi);

                printDocument.PrintPage += (sender, e) =>
                {
                    if (e.Graphics == null)
                    {
                        e.HasMorePages = false;
                        return;
                    }

                    int currentPageLabelIndex = 0;

                    while (currentImageIndex < images.Count && currentPageLabelIndex < labelsPerPage)
                    {
                        var row = currentPageLabelIndex / paperSettings.LabelsPerRow;
                        var col = currentPageLabelIndex % paperSettings.LabelsPerRow;

                        var x = col * labelWidthInch;
                        var y = row * labelHeightInch;

                        using var ms = new MemoryStream();
                        images[currentImageIndex].Save(ms, new BmpEncoder());
                        ms.Position = 0;

                        using var bitmap = new Bitmap(ms);
                        e.Graphics.DrawImage(bitmap, x, y, labelWidthInch, labelHeightInch);

                        currentImageIndex++;
                        currentPageLabelIndex++;
                    }

                    e.HasMorePages = currentImageIndex < images.Count;
                };

                printDocument.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка печати: {ex.Message}");
            }
        }

        private PrintDocument SetupPrintDocument(string printerName, PaperSettings paperSettings)
        {
            var printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = printerName;

            int paperWidth = ImageGenerationHelper.FromMmToHundredthsInch(paperSettings.Width);
            int paperHeight = ImageGenerationHelper.FromMmToHundredthsInch(paperSettings.Height);

            var paperSize = new PaperSize("Custom", paperWidth, paperHeight)
            {
                RawKind = (int)PaperKind.Custom
            };

            printDocument.DefaultPageSettings.PaperSize = paperSize;
            printDocument.DefaultPageSettings.Landscape = false;
            printDocument.PrinterSettings.DefaultPageSettings.Landscape = false;
            printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            return printDocument;
        }
    }
}