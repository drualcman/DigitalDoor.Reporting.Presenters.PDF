using DigitalDoor.Reporting.Presenters.PDF.Utilities;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Properties;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class TextMapperParagraph : TextMapperBase
{
    public Paragraph SetParagraph(string textValue, ColumnContent item, decimal height, decimal width)
    {
        Paragraph Text = new Paragraph();
        Text.SetMinHeight(MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Height + 1));
        Text.SetMinWidth(MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Width + 1));
        SetDimensions(Text, item.Column.Format);
        SetMargins(Text, item.Column.Format);
        SetPaddings(Text, item.Column.Format);
        SetBorders(Text, item.Column.Format);
        Color Color = GetColor(item.Column.Format.FontDetails.ColorSize.Colour.ToLower());
        if (item.Column.Format.FontDetails.FontStyle.Bold > 599)
        {
            Text.SetBold();
        }
        if (item.Column.Format.FontDetails.FontStyle.Italic)
        {
            Text.SetItalic();
        }
        Text.SetFontColor(Color);
        Text.SetFontSize((float)item.Column.Format.FontDetails.ColorSize.Width);
        try
        {

            PdfFont Font;
            string FontName = item.Column.Format.FontDetails.FontName;
            if (StandardFonts.IsStandardFont(FontName) || FontName == "Arial")
            {
                Font = FontName switch
                {
                    "Arial" => PdfFontFactory.CreateFont(StandardFonts.HELVETICA),
                    _ => PdfFontFactory.CreateFont(FontName)
                };
                Text.SetFont(Font);
            }
            else
            {
                if (FontService.ReportFont != null)
                {
                    byte[] BytesFont = FontService.ReportFont.GetFontBytesArray(FontName);
                    Font = PdfFontFactory.CreateFont(BytesFont, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED);
                    Text.SetFont(Font);
                }
            }
        }
        catch { }
        TextAlignment Aligment = item.Column.Format.TextAlignment switch
        {
            Report.TextAlignment.Right => TextAlignment.RIGHT,
            Report.TextAlignment.Center => TextAlignment.CENTER,
            Report.TextAlignment.Justify => TextAlignment.JUSTIFIED,
            _ => TextAlignment.LEFT,
        };
        Text.SetTextAlignment(Aligment);
        Text.SetFixedPosition(MillimeterMath.MillimeterToPixel(item.Column.Format.Position.Left + width),
                      MillimeterMath.MillimeterToPixel(height - (item.Column.Format.Position.Top + (decimal)(item.Column.Format.FontDetails.ColorSize.Width * 0.53))),
                      MillimeterMath.MillimeterToPixel(item.Column.Format.Dimension.Width));
        Text.SetRotationAngle(ConvertAngleToRadian(item.Column.Format.Angle));
        if (item.Column.Format.Background.ToLower() != "transparent")
        {
            Text.SetBackgroundColor(GetColor(item.Column.Format.Background));
        }
        Text.SetOpacity(item.Column.Format.FontDetails.ColorSize.Opacity);
        Text.Add(textValue);
        return Text;
    }
}
