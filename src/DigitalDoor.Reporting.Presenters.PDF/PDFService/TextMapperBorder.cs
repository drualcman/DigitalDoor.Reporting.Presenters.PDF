using DigitalDoor.Reporting.Entities.Models;
using iText.Layout.Element;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class TextMapperBorder : TextMapperBase
{
    public Div SetBorder(ColumnSetup item, decimal height, decimal weight)
    {
        Div Div = new Div();
        SetDimensions(Div, item.Format);
        SetMargins(Div, item.Format);
        SetPaddings(Div, item.Format);
        SetBorders(Div, item.Format);

        Div.SetFixedPosition(MillimeterMath.MillimeterToPixel(item.Format.Position.Left + weight),
                      MillimeterMath.MillimeterToPixel(height - item.Format.Position.Top), MillimeterMath.MillimeterToPixel(item.Format.Dimension.Width));
        if (item.Format.Borders.Bottom.Width > 0 ||
            item.Format.Borders.Top.Width > 0 ||
            item.Format.Borders.Left.Width > 0 ||
            item.Format.Borders.Right.Width > 0)
        {
            Div.SetFixedPosition(MillimeterMath.MillimeterToPixel(item.Format.Position.Left + weight),
                      MillimeterMath.MillimeterToPixel(height - (decimal)item.Format.Dimension.Height - item.Format.Position.Top),
                      MillimeterMath.MillimeterToPixel(item.Format.Dimension.Width));
        }
        Div.SetRotationAngle(ConvertAngleToRadian(item.Format.Angle));
        return Div;
    }
}
