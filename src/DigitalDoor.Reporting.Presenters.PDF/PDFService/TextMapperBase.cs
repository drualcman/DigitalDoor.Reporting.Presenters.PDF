using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class TextMapperBase : TextHelper
{
    protected Color GetColor(string color)
    {
        RgbColors drawColor = ColorTranslatorHelper.GetColor(color);
        return new DeviceRgb(drawColor.R,drawColor.G,drawColor.B);
    }

    protected iText.Layout.Borders.Border GetBorder(BorderStyle style, double width, string color)
    {
        return style switch
        {
            BorderStyle.dashed => new DashedBorder(GetColor(color), (float)width),
            BorderStyle.@double => new DoubleBorder(GetColor(color), (float)width),
            BorderStyle.groove => new GrooveBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.dotted => new DottedBorder(GetColor(color), (float)width),
            BorderStyle.outset => new OutsetBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.inset => new InsetBorder((DeviceRgb)GetColor(color), (float)width),
            BorderStyle.ridge => new RidgeBorder((DeviceRgb)GetColor(color), (float)width),
            _ => new SolidBorder(GetColor(color), (float)width)
        };
    }

    protected void SetBorders<T>(AbstractElement<T> element, Format format) where T : AbstractElement<T>
    {
        BorderStyle style = format.Borders.Style;
        if (format.Borders.Bottom.Width > 0)
            element.SetBorderBottom(GetBorder(style, MillimeterMath.MillimeterToPixel(format.Borders.Bottom.Width), format.Borders.Bottom.Colour));
        if (format.Borders.Top.Width > 0)
            element.SetBorderTop(GetBorder(style, MillimeterMath.MillimeterToPixel(format.Borders.Top.Width), format.Borders.Top.Colour));
        if (format.Borders.Right.Width > 0)
            element.SetBorderRight(GetBorder(style, MillimeterMath.MillimeterToPixel(format.Borders.Right.Width), format.Borders.Right.Colour));
        if (format.Borders.Left.Width > 0)
            element.SetBorderLeft(GetBorder(style, MillimeterMath.MillimeterToPixel(format.Borders.Left.Width), format.Borders.Left.Colour));
        SetRadius(element, format);
    }

    public double ConvertAngleToRadian(double angle)
    {
        return angle * (Math.PI / -180.0);
    }

    public void DrawBackground(Document page, string color, int positionPage, double heightBackground, decimal top)
    {
        if (color.ToLower() != "transparent" && !string.IsNullOrEmpty(color))
        {
            Div Background = new Div();
            Background.SetBackgroundColor(GetColor(color));
            Background.SetHeight(MillimeterMath.MillimeterToPixel(heightBackground));
            Background.SetPageNumber(positionPage);
            Background.SetFixedPosition(0, MillimeterMath.MillimeterToPixel(top), page.GetPdfDocument().GetDefaultPageSize().GetWidth());
            page.Add(Background);
        }
    }
}
