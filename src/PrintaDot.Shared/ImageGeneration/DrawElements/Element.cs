namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class Element
{
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
}
