using DigitalDoor.Reporting.Entities.ValueObjects;
using iText.IO.Image;
using iText.Layout.Element;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class TextMapperImage : TextMapperBase
{
    public Image SetImage(byte[] bytes, ColumnContent item, decimal height, decimal weight)
    {
        Image Image = null;
        try
        {
            ImageData imageData = ImageDataFactory.Create(bytes);
            Image = new Image(imageData);
            SetRadius(Image, item.Column.Format);
            Image.SetPaddingTop(MillimeterMath.MillimeterToPixel(item.Column.Format.Padding.Top));
            Image.SetPaddingBottom(MillimeterMath.MillimeterToPixel(item.Column.Format.Padding.Bottom));
            Image.SetPaddingLeft(MillimeterMath.MillimeterToPixel(item.Column.Format.Padding.Left));
            Image.SetPaddingRight(MillimeterMath.MillimeterToPixel(item.Column.Format.Padding.Right));
            Image.SetMarginTop(MillimeterMath.MillimeterToPixel(item.Column.Format.Margin.Top));
            Image.SetMarginBottom(MillimeterMath.MillimeterToPixel(item.Column.Format.Margin.Bottom));
            Image.SetMarginLeft(MillimeterMath.MillimeterToPixel(item.Column.Format.Margin.Left));
            Image.SetMarginRight(MillimeterMath.MillimeterToPixel(item.Column.Format.Margin.Right));
            Image.SetHeight(MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Height));
            Image.SetWidth(MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Width));
            Image.SetFixedPosition(MillimeterMath.MillimeterToPixel(item.Column.Format.Position.Left + weight),
                    MillimeterMath.MillimeterToPixel(height - item.Column.Format.Position.Top - (decimal)item.Column.Format.Dimension.Height),
                    MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Width));
            Image.SetRotationAngle(ConvertAngleToRadian(item.Column.Format.Angle));
        }
        catch { }
        return Image;
    }
}
