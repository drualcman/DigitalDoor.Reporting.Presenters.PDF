namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal readonly struct TextLine
{
    public string Text { get; }
    public bool IsLastLineOfParagraph { get; }

    public TextLine(string text, bool isLastLineOfParagraph)
    {
        Text = text;
        IsLastLineOfParagraph = isLastLineOfParagraph;
    }
}
