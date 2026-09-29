namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal static class TextLineDrawer
{
    private static readonly char[] WordSeparator = { ' ' };

    public static DrawnTextSpan DrawLine(SKCanvas canvas, TextLine line, SKRect contentBox, float baseline,
        TextAlignment alignment, SKFont font, SKPaint paint)
    {
        DrawnTextSpan drawnSpan;
        string[] words = line.Text.Split(WordSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (alignment == TextAlignment.Justify && !line.IsLastLineOfParagraph && words.Length > 1)
        {
            drawnSpan = DrawJustifiedLine(canvas, words, contentBox, baseline, font, paint);
        }
        else
        {
            float lineWidth = font.MeasureText(line.Text);
            float startX = GetAlignedStartX(contentBox, lineWidth, alignment);
            canvas.DrawText(line.Text, startX, baseline, SKTextAlign.Left, font, paint);
            drawnSpan = new DrawnTextSpan(startX, lineWidth);
        }
        return drawnSpan;
    }

    private static DrawnTextSpan DrawJustifiedLine(SKCanvas canvas, string[] words, SKRect contentBox, float baseline,
        SKFont font, SKPaint paint)
    {
        float[] wordWidths = words.Select(word => font.MeasureText(word)).ToArray();
        float gapBetweenWords = (contentBox.Width - wordWidths.Sum()) / (words.Length - 1);
        float currentX = contentBox.Left;
        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            canvas.DrawText(words[wordIndex], currentX, baseline, SKTextAlign.Left, font, paint);
            currentX += wordWidths[wordIndex] + gapBetweenWords;
        }
        return new DrawnTextSpan(contentBox.Left, contentBox.Width);
    }

    private static float GetAlignedStartX(SKRect contentBox, float lineWidth, TextAlignment alignment)
    {
        float startX = alignment switch
        {
            TextAlignment.Right => contentBox.Right - lineWidth,
            TextAlignment.Center => contentBox.Left + (contentBox.Width - lineWidth) / 2f,
            _ => contentBox.Left
        };
        return startX;
    }
}
