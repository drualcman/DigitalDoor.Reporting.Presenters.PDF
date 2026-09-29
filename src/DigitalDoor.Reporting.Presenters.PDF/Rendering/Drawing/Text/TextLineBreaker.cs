namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal static class TextLineBreaker
{
    private static readonly char[] ParagraphSeparators = { '\n' };
    private static readonly char[] WordSeparators = { ' ', '\t' };

    public static List<TextLine> BreakIntoLines(string text, SKFont font, float maximumLineWidth)
    {
        List<TextLine> lines = new List<TextLine>();
        string normalizedText = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
        foreach (string paragraph in normalizedText.Split(ParagraphSeparators))
        {
            List<string> paragraphLines = BreakParagraph(paragraph, font, maximumLineWidth);
            for (int lineIndex = 0; lineIndex < paragraphLines.Count; lineIndex++)
            {
                lines.Add(new TextLine(paragraphLines[lineIndex], lineIndex == paragraphLines.Count - 1));
            }
        }
        return lines;
    }

    private static List<string> BreakParagraph(string paragraph, SKFont font, float maximumLineWidth)
    {
        List<string> paragraphLines = new List<string>();
        string currentLine = string.Empty;
        foreach (string word in paragraph.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries))
        {
            string candidateLine = currentLine.Length == 0 ? word : $"{currentLine} {word}";
            if (maximumLineWidth <= 0 || font.MeasureText(candidateLine) <= maximumLineWidth)
            {
                currentLine = candidateLine;
            }
            else
            {
                if (currentLine.Length > 0)
                {
                    paragraphLines.Add(currentLine);
                }
                currentLine = SplitLongWord(word, font, maximumLineWidth, paragraphLines);
            }
        }
        paragraphLines.Add(currentLine);
        return paragraphLines;
    }

    private static string SplitLongWord(string word, SKFont font, float maximumLineWidth, List<string> paragraphLines)
    {
        string remainingWord = word;
        while (remainingWord.Length > 1 && font.MeasureText(remainingWord) > maximumLineWidth)
        {
            int fittingCharacters = CountFittingCharacters(remainingWord, font, maximumLineWidth);
            paragraphLines.Add(remainingWord.Substring(0, fittingCharacters));
            remainingWord = remainingWord.Substring(fittingCharacters);
        }
        return remainingWord;
    }

    private static int CountFittingCharacters(string word, SKFont font, float maximumLineWidth)
    {
        int fittingCharacters = 1;
        bool nextCharacterFits = true;
        while (fittingCharacters < word.Length && nextCharacterFits)
        {
            nextCharacterFits = font.MeasureText(word.Substring(0, fittingCharacters + 1)) <= maximumLineWidth;
            if (nextCharacterFits)
            {
                fittingCharacters++;
            }
        }
        return fittingCharacters;
    }
}
