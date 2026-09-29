namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class SideBorderDrawer
{
    public static void Draw(SKCanvas canvas, SKRect box, BorderStyle style, BorderSide side, float borderWidth, SKColor baseColor)
    {
        foreach (BorderBand band in BorderStyleBands.GetBands(style, borderWidth))
        {
            float bandStart = band.StartFraction * borderWidth;
            float bandEnd = band.EndFraction * borderWidth;
            float bandCenterOffset = (bandStart + bandEnd) / 2f;
            SKColor bandColor = BorderToneCalculator.GetSideColor(baseColor, band.Tone, side);
            using SKPaint paint = BorderPaintFactory.CreateStrokePaint(style, bandColor, bandEnd - bandStart, borderWidth);
            DrawSideLine(canvas, box, side, bandCenterOffset, paint);
        }
    }

    private static void DrawSideLine(SKCanvas canvas, SKRect box, BorderSide side, float offsetFromEdge, SKPaint paint)
    {
        if (side == BorderSide.Top)
        {
            canvas.DrawLine(box.Left, box.Top + offsetFromEdge, box.Right, box.Top + offsetFromEdge, paint);
        }
        else if (side == BorderSide.Bottom)
        {
            canvas.DrawLine(box.Left, box.Bottom - offsetFromEdge, box.Right, box.Bottom - offsetFromEdge, paint);
        }
        else if (side == BorderSide.Left)
        {
            canvas.DrawLine(box.Left + offsetFromEdge, box.Top, box.Left + offsetFromEdge, box.Bottom, paint);
        }
        else
        {
            canvas.DrawLine(box.Right - offsetFromEdge, box.Top, box.Right - offsetFromEdge, box.Bottom, paint);
        }
    }
}
