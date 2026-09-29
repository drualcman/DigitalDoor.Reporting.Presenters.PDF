namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal readonly struct BorderBand
{
    public float StartFraction { get; }
    public float EndFraction { get; }
    public BorderTone Tone { get; }

    public BorderBand(float startFraction, float endFraction, BorderTone tone)
    {
        StartFraction = startFraction;
        EndFraction = endFraction;
        Tone = tone;
    }
}
