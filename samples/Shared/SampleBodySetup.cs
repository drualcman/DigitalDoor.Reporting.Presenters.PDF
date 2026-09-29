using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleBodySetup
{
    public const double RowHeight = 20;
    public const int ColumnsSpace = 6;

    public static void Configure(Section body, double contentWidth, double bodyHeight, int columnsNumber)
    {
        double rowWidth = (contentWidth - ColumnsSpace * (columnsNumber - 1)) / columnsNumber;
        body.Format.Dimension = new Dimension(contentWidth, bodyHeight);
        body.ColumnsNumber = columnsNumber;
        body.ColumnsSpace = ColumnsSpace;
        body.Row = new Row(new Dimension(rowWidth, RowHeight), new Border(new Shade(0.2, "lightgray"), BorderStyle.solid));

        body.AddColumn(SampleColumns.RowIndex, SampleFormats.CreateTextFormat(2, 2, 9, 5, 8, fontWeight: 700, colour: "#1565C0"));
        body.AddColumn(SampleColumns.RowName, SampleFormats.CreateTextFormat(11, 2, rowWidth - 43, 5, 9, fontWeight: 600));

        Format amount = SampleFormats.CreateBoxFormat((decimal)rowWidth - 30, 2, 28, 5, "#FFF8E1", BorderStyle.solid, 0.3, "#FF8F00", 1);
        amount.TextAlignment = TextAlignment.Right;
        amount.Padding = new Kernel(0, 1, 0, 1);
        amount.FontDetails = new Font("Arial", new Shade(8, "#E65100"), new FontStyle(400));
        body.AddColumn(SampleColumns.RowAmount, amount);

        Format description = SampleFormats.CreateTextFormat(2, 8, rowWidth - 30, 11, 7, TextAlignment.Justify, colour: "#333333");
        body.AddColumn(SampleColumns.RowDescription, description);

        Format thumbnail = SampleFormats.CreateImageFormat((decimal)rowWidth - 24, 8, 20, 11);
        thumbnail.Borders = new Border(new Shade(0.3, "#9E9E9E", new Kernel(1.5m)), BorderStyle.solid);
        body.AddColumn(SampleColumns.RowThumbnail, thumbnail);
    }
}
