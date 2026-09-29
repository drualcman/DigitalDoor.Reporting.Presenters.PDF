namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal static class TextCellDrawer
{
    private const string DefaultTextColour = "black";

    public static void Draw(SKCanvas canvas, string text, SKRect clipBox, SKRect contentBox, Format format, ReportTypefaceProvider typefaces)
    {
        Font fontDetails = format.FontDetails ?? new Font();
        ResolvedTypeface typeface = typefaces.GetTypeface(fontDetails);
        using SKFont font = ReportFontFactory.CreateFont(typeface, fontDetails);
        SKColor textColor = ColorTranslatorHelper.ConvertToSKColor(fontDetails.ColorSize?.Colour ?? DefaultTextColour);
        using SKPaint textPaint = new SKPaint { Color = textColor, IsAntialias = true, Style = SKPaintStyle.Fill };
        List<TextLine> lines = TextLineBreaker.BreakIntoLines(text, font, contentBox.Width);
        SKFontMetrics metrics = font.Metrics;
        float lineHeight = font.Size * (float)ReportCellLayout.LineHeightFactor;
        float halfLeading = (lineHeight - (metrics.Descent - metrics.Ascent)) / 2f;
        int saveCount = canvas.Save();
        canvas.ClipRect(clipBox);
        int lineIndex = 0;
        while (lineIndex < lines.Count && contentBox.Top + lineIndex * lineHeight < clipBox.Bottom)
        {
            float baseline = contentBox.Top + lineIndex * lineHeight + halfLeading - metrics.Ascent;
            DrawnTextSpan span = TextLineDrawer.DrawLine(canvas, lines[lineIndex], contentBox, baseline, format.TextAlignment, font, textPaint);
            TextDecorationDrawer.Draw(canvas, format.TextDecoration, span, baseline, font, textColor);
            lineIndex++;
        }
        canvas.RestoreToCount(saveCount);
    }
}
