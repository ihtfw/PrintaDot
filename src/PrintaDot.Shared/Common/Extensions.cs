using PrintaDot.Shared.NativeMessaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace PrintaDot.Shared.Common;

public static class Extensions
{
    /// <summary>
    /// Checks if the host is registered with all required browsers.
    /// </summary>
    /// <returns><see langword="true"/> if the required information is present in the registry.</returns>
    public static bool IsAnyRegistered(this IEnumerable<Browser> browsers, string hostName, string manifestPath)
    {
        bool result = false;

        foreach (Browser browser in browsers)
        {
            result = result || browser.IsRegistered(hostName, manifestPath);
        }

        return result;
    }

    /// <summary>
    /// Register the application to open with all required browsers.
    /// </summary>
    public static void Register(this IEnumerable<Browser> browsers, string hostName, string manifestPath)
    {
        foreach (Browser browser in browsers)
        {
            browser.Register(hostName, manifestPath);
        }
    }

    /// <summary>
    /// De-register the application to open with all required browsers.
    /// </summary>
    public static void Unregister(this IEnumerable<Browser> browsers, string hostName)
    {
        foreach (Browser browser in browsers)
        {
            browser.Unregister(hostName);
        }
    }

    /// <summary>
    /// Needs for drawing bounds of element.
    /// </summary>
    /// <param name="ctx">Image processing context</param>
    /// <param name="x">Top left X of element</param>
    /// <param name="y">Top left Y point of element</param>
    /// <param name="width">Width of element</param>
    /// <param name="height">Height of element</param>
    public static void DrawBbox(this IImageProcessingContext ctx, float x, float y, float width, float height)
    {
        float strokeWidth = 1f;
        var rectangle = new RectangularPolygon(
        x,
        y,
            width - strokeWidth,
            height - strokeWidth
        );

        ctx.Draw(Color.Red, strokeWidth, rectangle);
    }
}
