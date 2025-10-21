using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration;

public class BarcodeLabelFigure
{
    public PointF BarcodeTopLeft  { get; set; }
    public PointF HeaderTopLeft { get; set; }
    public PointF FiguresTopLeft { get; set; }
}
