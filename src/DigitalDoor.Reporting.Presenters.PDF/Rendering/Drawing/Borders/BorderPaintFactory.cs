namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class BorderPaintFactory
{
    private const float DashLengthMultiplier = 3f;
    private const float DashGapMultiplier = 2f;
    private const float DotGapMultiplier = 2f;
    private const float MinimumPatternLength = 0.5f;
    private const float DotDashLength = 0.01f;

    public static SKPaint CreateStrokePaint(BorderStyle style, SKColor color, float strokeWidth, float fullBorderWidth)
    {
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true
        };
        float patternUnit = Math.Max(fullBorderWidth, MinimumPatternLength);
        if (style == BorderStyle.dashed)
        {
            paint.PathEffect = SKPathEffect.CreateDash(new[] { patternUnit * DashLengthMultiplier, patternUnit * DashGapMultiplier }, 0);
        }
        else if (style == BorderStyle.dotted)
        {
            paint.StrokeCap = SKStrokeCap.Round;
            paint.PathEffect = SKPathEffect.CreateDash(new[] { DotDashLength, patternUnit * DotGapMultiplier }, 0);
        }
        return paint;
    }
}
