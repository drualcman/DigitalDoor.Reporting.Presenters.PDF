namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal sealed class SvgTextRun
{
    public SvgTextRun(Font font, string text)
    {
        Font = font;
        Text = text;
    }

    public Font Font { get; }
    public string Text { get; }
}
