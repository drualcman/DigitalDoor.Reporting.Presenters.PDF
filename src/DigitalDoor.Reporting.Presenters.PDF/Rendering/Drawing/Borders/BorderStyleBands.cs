namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class BorderStyleBands
{
    private const float OneThird = 1f / 3f;
    private const float TwoThirds = 2f / 3f;
    private const float Half = 0.5f;
    private const float MinimumDoubleBorderWidthInPoints = 2.25f;

    public static IReadOnlyList<BorderBand> GetBands(BorderStyle style, float borderWidthInPoints)
    {
        BorderStyle drawnStyle = style == BorderStyle.@double && borderWidthInPoints < MinimumDoubleBorderWidthInPoints ? BorderStyle.solid : style;
        IReadOnlyList<BorderBand> bands = drawnStyle switch
        {
            BorderStyle.hidden => Array.Empty<BorderBand>(),
            BorderStyle.@double => new[] { new BorderBand(0f, OneThird, BorderTone.Base), new BorderBand(TwoThirds, 1f, BorderTone.Base) },
            BorderStyle.inset => new[] { new BorderBand(0f, 1f, BorderTone.Inset) },
            BorderStyle.outset => new[] { new BorderBand(0f, 1f, BorderTone.Outset) },
            BorderStyle.groove => new[] { new BorderBand(0f, Half, BorderTone.Inset), new BorderBand(Half, 1f, BorderTone.Outset) },
            BorderStyle.ridge => new[] { new BorderBand(0f, Half, BorderTone.Outset), new BorderBand(Half, 1f, BorderTone.Inset) },
            _ => new[] { new BorderBand(0f, 1f, BorderTone.Base) }
        };
        return bands;
    }

    public static bool UsesSideTones(BorderStyle style)
    {
        return style == BorderStyle.inset || style == BorderStyle.outset ||
            style == BorderStyle.groove || style == BorderStyle.ridge;
    }
}
