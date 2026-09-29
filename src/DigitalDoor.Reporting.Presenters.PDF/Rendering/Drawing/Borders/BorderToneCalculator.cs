namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class BorderToneCalculator
{
    private const float ToneBlendFactor = 0.4f;

    public static SKColor GetSideColor(SKColor baseColor, BorderTone tone, BorderSide side)
    {
        bool isTopOrLeft = side == BorderSide.Top || side == BorderSide.Left;
        SKColor sideColor = tone switch
        {
            BorderTone.Inset => isTopOrLeft ? Blend(baseColor, SKColors.Black) : Blend(baseColor, SKColors.White),
            BorderTone.Outset => isTopOrLeft ? Blend(baseColor, SKColors.White) : Blend(baseColor, SKColors.Black),
            _ => baseColor
        };
        return sideColor;
    }

    private static SKColor Blend(SKColor baseColor, SKColor targetColor)
    {
        return new SKColor(
            BlendChannel(baseColor.Red, targetColor.Red),
            BlendChannel(baseColor.Green, targetColor.Green),
            BlendChannel(baseColor.Blue, targetColor.Blue),
            baseColor.Alpha);
    }

    private static byte BlendChannel(byte baseChannel, byte targetChannel)
    {
        return (byte)Math.Round(baseChannel + (targetChannel - baseChannel) * ToneBlendFactor);
    }
}
