using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleFormats
{
    public static Format CreateTextFormat(decimal left, decimal top, double width, double height, int fontSize,
        TextAlignment alignment = TextAlignment.Left, int fontWeight = 400, bool italic = false, string colour = "black")
    {
        Format format = new Format(width, height)
        {
            Position = new Kernel(top, left),
            TextAlignment = alignment,
            FontDetails = new Font("Arial", new Shade(fontSize, colour), new FontStyle(fontWeight, italic))
        };
        return format;
    }

    public static Format CreateBoxFormat(decimal left, decimal top, double width, double height, string background,
        BorderStyle borderStyle, double borderWidth, string borderColour, decimal radius = 0)
    {
        Format format = new Format(width, height)
        {
            Position = new Kernel(top, left),
            Background = background,
            Borders = new Border(new Shade(borderWidth, borderColour, new Kernel(radius)), borderStyle)
        };
        return format;
    }

    public static Format CreateImageFormat(decimal left, decimal top, double width, double height, double angle = 0)
    {
        Format format = new Format(width, height)
        {
            Position = new Kernel(top, left),
            Angle = angle
        };
        return format;
    }
}
