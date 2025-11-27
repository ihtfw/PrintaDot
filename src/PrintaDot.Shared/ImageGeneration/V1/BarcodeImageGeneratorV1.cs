using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using PrintaDot.Shared.ImageGeneration.DrawElements;
using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol.V1.Requests;

namespace PrintaDot.Shared.ImageGeneration.V1;

public class BarcodeImageGeneratorV1 : IPrintaDotImageGenerator
{
    private List<PrintRequestMessageV1.Item> _items;
    private PixelImageProfileV1 _profile;

    public BarcodeImageGeneratorV1(PrintRequestMessageV1 message)
    {
        _profile = new PixelImageProfileV1(message.Profile);
        _items = message.Items;
    }

    public List<Image> GenerateImages()
    {
        var images = new List<Image>();

        foreach (var item in _items)
        {
            var labelImage = GenerateBackround((int)_profile.LabelWidth, (int)_profile.LabelHeight);
            var barcodeImage = GenerateBackround((int)_profile.LabelWidth, (int)_profile.LabelHeight);

            var barcodeElement = new BarcodeElement(_profile, item.Barcode);
            var headerElement = new HeaderElement(_profile, item.Header, barcodeElement.TopLeft);
            var figuresElement = new FiguresElement(_profile, item.Figures, barcodeElement.TopLeft, barcodeElement.BarcodeImage.Height);

            headerElement.Draw(barcodeImage);
            barcodeElement.Draw(barcodeImage);
            figuresElement.Draw(barcodeImage);

            images.Add(barcodeImage);
        }

        return images;
    }

    public Image<Rgba32> GenerateBackround(int width, int height)
    {
        var image = new Image<Rgba32>(width, height);

        image.Metadata.HorizontalResolution = ImageGenerationHelper.DEFAULT_DPI;
        image.Metadata.VerticalResolution = ImageGenerationHelper.DEFAULT_DPI;

        image.Mutate(ctx =>
        {
            ctx.BackgroundColor(Color.White);
#if DEBUG
            ctx.DrawBbox(0, 0, image.Width, image.Height);
#endif
        });

        return image;
    }
}