namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal static class TextDecorationDrawer
{
    private const float DefaultThicknessRatio = 1f / 15f;
    private const float DefaultUnderlineOffsetRatio = 0.1f;
    private const float DefaultStrikeoutOffsetRatio = -0.3f;

    public static void Draw(SKCanvas canvas, TextDecoration decoration, DrawnTextSpan span, float baseline, SKFont font, SKColor color)
    {
        if (decoration != TextDecoration.None && span.Width > 0)
        {
            SKFontMetrics metrics = font.Metrics;
            float thickness = metrics.UnderlineThickness ?? font.Size * DefaultThicknessRatio;
            float lineY = decoration switch
            {
                TextDecoration.Underline => baseline + (metrics.UnderlinePosition ?? font.Size * DefaultUnderlineOffsetRatio),
                TextDecoration.Line => baseline + (metrics.StrikeoutPosition ?? font.Size * DefaultStrikeoutOffsetRatio),
                _ => baseline + metrics.Ascent
            };
            using SKPaint decorationPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = Math.Max(thickness, 0.1f),
                Color = color,
                IsAntialias = true
            };
            canvas.DrawLine(span.StartX, lineY, span.StartX + span.Width, lineY, decorationPaint);
        }
    }
}
