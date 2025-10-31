using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class Element
{
    public PointF TopLeft { get; set; }
    public PointF Center { get; set; }
    public PointF Offset { get; set; }
    public float Rotation { get; set; }

    protected float GetHorizontalAligment(string aligment, float labelWdith, float elementWidth, float alligmentValue)
    {
        switch (aligment)
        {
            case "Left":
                return 0.0f;
            case "Right":
                return labelWdith - elementWidth;
            case "Center":
                return alligmentValue;
            case "Stretched":
                throw new NotImplementedException();
            default:
                return 0.0f;

        }
    }

    protected Point CalculateTopLeftRotated(Image rotatedImage, float originalWidth, float originalHeight)
    {
        var originalCenterX = TopLeft.X + originalWidth / 2f;
        var originalCenterY = TopLeft.Y + originalHeight / 2f;

        var newTopLeftX = originalCenterX - rotatedImage.Width / 2f;
        var newTopLeftY = originalCenterY - rotatedImage.Height / 2f;

        return new Point((int)(newTopLeftX + Offset.X), (int)(newTopLeftY + Offset.Y));
    }
}
