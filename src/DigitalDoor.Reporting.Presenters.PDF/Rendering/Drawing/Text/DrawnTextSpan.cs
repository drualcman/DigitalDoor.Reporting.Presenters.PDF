namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Text;

internal readonly struct DrawnTextSpan
{
    public float StartX { get; }
    public float Width { get; }

    public DrawnTextSpan(float startX, float width)
    {
        StartX = startX;
        Width = width;
    }
}
