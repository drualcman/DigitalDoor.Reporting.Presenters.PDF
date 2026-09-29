namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing;

internal static class BackgroundDrawer
{
    public static void Draw(SKCanvas canvas, SKRect box, string background, Border borders)
    {
        if (!ColorTranslatorHelper.IsTransparent(background) && box.Width > 0 && box.Height > 0)
        {
            using SKPaint backgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = ColorTranslatorHelper.ConvertToSKColor(background),
                IsAntialias = true
            };
            using SKRoundRect roundedBox = CellGeometry.GetRoundedBox(box, borders);
            canvas.DrawRoundRect(roundedBox, backgroundPaint);
        }
    }
}
