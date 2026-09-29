using System.Globalization;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleReportDataFactory
{
    private const string LongDescription =
        "Descripción larga con acentos (áéíóú, ñ, ç) y símbolos € — para comprobar el ajuste de líneas " +
        "dentro del ancho y del alto de la celda. Este texto debe cortarse cuando ya no quepa en la caja.";

    public static List<ColumnData> CreateData(int bodyRows, bool includePlainBase64Strings)
    {
        List<ColumnData> data = new List<ColumnData>();
        AddHeaderData(data);
        AddFooterData(data);
        byte[] pngLogo = SampleImages.CreatePngLogo();
        byte[] jpegPhoto = SampleImages.CreateJpegPhoto();
        string base64Png = SampleImages.CreateBase64Png();
        NumberFormatInfo spanishNumberFormat = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };
        for (int rowNumber = 1; rowNumber <= bodyRows; rowNumber++)
        {
            data.Add(Body(SampleColumns.RowIndex, $"#{rowNumber}", rowNumber));
            data.Add(Body(SampleColumns.RowName, $"Producto número {rowNumber}", rowNumber));
            data.Add(Body(SampleColumns.RowAmount, $"{(rowNumber * 1234.5m).ToString("N2", spanishNumberFormat)} €", rowNumber));
            data.Add(Body(SampleColumns.RowDescription, LongDescription, rowNumber));
            object thumbnail = SelectThumbnail(rowNumber, pngLogo, jpegPhoto, base64Png, includePlainBase64Strings);
            if (thumbnail is not null)
            {
                data.Add(Body(SampleColumns.RowThumbnail, thumbnail, rowNumber));
            }
        }
        return data;
    }

    private static object SelectThumbnail(int rowNumber, byte[] pngLogo, byte[] jpegPhoto, string base64Png, bool includePlainBase64Strings)
    {
        object thumbnail = (rowNumber % 5) switch
        {
            0 => pngLogo,
            1 => jpegPhoto,
            2 => SampleImages.VectorBadgeSvg,
            3 => includePlainBase64Strings ? base64Png : pngLogo,
            _ => null
        };
        return thumbnail;
    }

    private static void AddHeaderData(List<ColumnData> data)
    {
        data.Add(Header(SampleColumns.HeaderBackground, " "));
        data.Add(Header(SampleColumns.HeaderTitle, "DigitalDoor Reporting — comparativa PDF"));
        data.Add(Header(SampleColumns.HeaderSubtitle, "Cabecera repetida en cada página con logo PNG y SVG"));
        data.Add(Header(SampleColumns.HeaderLogo, SampleImages.CreatePngLogo()));
        data.Add(Header(SampleColumns.HeaderBadge, SampleImages.VectorBadgeSvg));
        data.Add(Header(SampleColumns.HeaderPageLabel, "Página"));
        data.Add(Header(SampleColumns.HeaderOfLabel, "de"));
    }

    private static void AddFooterData(List<ColumnData> data)
    {
        data.Add(Footer(SampleColumns.FooterLine, " "));
        data.Add(Footer(SampleColumns.FooterNote, "Pie de página subrayado · generado por DigitalDoor.Reporting"));
        data.Add(Footer(SampleColumns.FooterStrike, "Texto tachado de ejemplo"));
        data.Add(Footer(SampleColumns.FooterWatermark, "CONFIDENCIAL"));
        data.Add(Footer(SampleColumns.FooterPageLabel, "Página"));
        data.Add(Footer(SampleColumns.FooterOfLabel, "/"));
    }

    private static ColumnData Header(string column, object value) =>
        new ColumnData(new Item(column), value, SectionType.Header);

    private static ColumnData Footer(string column, object value) =>
        new ColumnData(new Item(column), value, SectionType.Footer);

    private static ColumnData Body(string column, object value, int rowNumber) =>
        new ColumnData(new Item(column), value, SectionType.Body) { Row = rowNumber };
}
