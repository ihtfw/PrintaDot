using System.Drawing.Printing;
using System.Drawing;
using PrintaDot.Shared.Platform;
using SixLabors.ImageSharp.Formats.Bmp;
using PrintaDot.Shared.ImageGeneration;
using PrintaDot.Shared.Printing;
using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.Common;

namespace PrintaDot.Windows;

public class WindowsPrintingService : IPlatformPrintingService
{
    private string PrinterByDefault => "Microsoft Print to PDF";
    public bool Print(string printerName, List<SixLabors.ImageSharp.Image> images, PaperSettings paperSettings)
    {
        try
        {
            using var printDocument = SetupPrintDocument(printerName, paperSettings);

            var currentImageIndex = 0;
            var currentPageNumber = 0;

            //caculating size of each label
            var dpi = (float)images.First().Metadata.HorizontalResolution;
            var labelWidthInch = ImageGenerationHelper.FromPixelsToHundredthsInch(images[0].Width, dpi);
            var labelHeightInch = ImageGenerationHelper.FromPixelsToHundredthsInch(images[0].Height, dpi);

            //calculating size of page
            var paperWidthInch = printDocument.DefaultPageSettings.PaperSize.Width;
            var paperHeightInch = printDocument.DefaultPageSettings.PaperSize.Height;

            var (labelsPerRow, labelsPerColumn) = CalculateLabelsPerPage(paperSettings, labelWidthInch, labelHeightInch, paperWidthInch, paperHeightInch);
            var labelsPerPage = labelsPerRow * labelsPerColumn;

            var offset = paperSettings.Offset ?? 0;
            var repeat = paperSettings.Repeat ?? 1;

            var extendedImages = new List<SixLabors.ImageSharp.Image>();
            for (int i = 0; i < repeat; i++)
            {
                extendedImages.AddRange(images);
            }

            printDocument.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null)
                {
                    e.HasMorePages = false;
                    return;
                }

                int currentPageLabelIndex = 0;

                var currentOffset = currentPageNumber == 0 ? offset : 0;

                while (currentImageIndex < extendedImages.Count && currentPageLabelIndex < labelsPerPage)
                {
                    var effectivePosition = currentPageLabelIndex + currentOffset;

                    if (effectivePosition >= labelsPerPage)
                    {
                        break;
                    }

                    var row = effectivePosition / labelsPerRow;
                    var col = effectivePosition % labelsPerRow;

                    var x = col * labelWidthInch;
                    var y = row * labelHeightInch;

                    using var ms = new MemoryStream();
                    extendedImages[currentImageIndex].Save(ms, new BmpEncoder());
                    ms.Position = 0;

                    using var bitmap = new Bitmap(ms);
                    e.Graphics.DrawImage(bitmap, x, y, labelWidthInch, labelHeightInch);

                    currentImageIndex++;
                    currentPageLabelIndex++;
                }

                currentPageNumber++;
                e.HasMorePages = currentImageIndex < extendedImages.Count;
            };

            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                printDocument.PrinterSettings.PrinterName = PrinterByDefault;
                printDocument.Print();
            }  
        }
        catch (Exception ex)
        {
            Log.LogMessage(ex.Message, nameof(WindowsPrintingService));

            return false;
        }

        return true;
    }

    private (int labelsPerRow, int labelsPerColumn) CalculateLabelsPerPage(
        PaperSettings paperSettings,
        float labelWidthInch,
        float labelHeightInch,
        int paperWidthInch,
        int paperHeightInch)
    {
        //if we have values use them
        if (paperSettings.LabelsPerRow > 0 && paperSettings.LabelsPerColumn > 0)
        {
            return (paperSettings.LabelsPerRow, paperSettings.LabelsPerColumn);
        }

        //calculating labels per row
        int calculatedLabelsPerRow = paperSettings.LabelsPerRow > 0
            ? paperSettings.LabelsPerRow
            : (int)Math.Floor(paperWidthInch / labelWidthInch);
        //calculating labels per column
        int calculatedLabelsPerColumn = paperSettings.LabelsPerColumn > 0
            ? paperSettings.LabelsPerColumn
            : (int)Math.Floor(paperHeightInch / labelHeightInch);


        calculatedLabelsPerRow = Math.Max(1, calculatedLabelsPerRow);
        calculatedLabelsPerColumn = Math.Max(1, calculatedLabelsPerColumn);

        return (calculatedLabelsPerRow, calculatedLabelsPerColumn);
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

    public GetPrintersResponse GetInstalledPrinters(Guid messageId)
    {
        var printers = new List<string>();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printers.Add(printer);
        }

        return new GetPrintersResponse()
        {
            MessageIdToResponse = messageId,
            Type = ResponseType.GetPrintersResponse,
            Version = 1,
            Printers = printers
        };
    }
}