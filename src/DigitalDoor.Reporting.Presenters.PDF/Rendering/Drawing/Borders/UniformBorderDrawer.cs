namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class UniformBorderDrawer
{
    public static void Draw(SKCanvas canvas, SKRect box, Border borders, BorderStyle style, float borderWidth, SKColor color)
    {
        foreach (BorderBand band in BorderStyleBands.GetBands(style, borderWidth))
        {
            float bandStart = band.StartFraction * borderWidth;
            float bandEnd = band.EndFraction * borderWidth;
            float bandCenterOffset = (bandStart + bandEnd) / 2f;
            using SKRoundRect roundedBox = CellGeometry.GetRoundedBox(box, borders);
            using SKRoundRect bandPath = new SKRoundRect(roundedBox);
            bandPath.Deflate(bandCenterOffset, bandCenterOffset);
            using SKPaint paint = BorderPaintFactory.CreateStrokePaint(style, color, bandEnd - bandStart, borderWidth);
            canvas.DrawRoundRect(bandPath, paint);
        }
    }
}
