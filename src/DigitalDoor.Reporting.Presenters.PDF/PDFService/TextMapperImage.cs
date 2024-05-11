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
            Image.SetPaddingTop(MillimeterToPixel(item.Column.Format.Padding.Top));
            Image.SetPaddingBottom(MillimeterToPixel(item.Column.Format.Padding.Bottom));
            Image.SetPaddingLeft(MillimeterToPixel(item.Column.Format.Padding.Left));
            Image.SetPaddingRight(MillimeterToPixel(item.Column.Format.Padding.Right));
            Image.SetMarginTop(MillimeterToPixel(item.Column.Format.Margin.Top));
            Image.SetMarginBottom(MillimeterToPixel(item.Column.Format.Margin.Bottom));
            Image.SetMarginLeft(MillimeterToPixel(item.Column.Format.Margin.Left));
            Image.SetMarginRight(MillimeterToPixel(item.Column.Format.Margin.Right));
            Image.SetHeight(MillimeterToPixel(item.Column.Format.Dimension.Height));
            Image.SetWidth(MillimeterToPixel(item.Column.Format.Dimension.Width));
            Image.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left + weight),
                    MillimeterToPixel(height - item.Column.Format.Position.Top - (decimal)item.Column.Format.Dimension.Height),
                    MillimeterToPixel(item.Column.Format.Dimension.Width));
            Image.SetRotationAngle(ConvertAngleToRadian(item.Column.Format.Angle));
        }
        catch { }
        return Image;
    }
}
