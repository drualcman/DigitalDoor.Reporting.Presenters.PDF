using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleFooterSetup
{
    public const double FooterHeight = 15;

    public static void Configure(Section footer, double contentWidth)
    {
        footer.Format.Dimension = new Dimension(contentWidth, FooterHeight);
        footer.Format.Background = "#FAFAFA";
        footer.Row = new Row(new Dimension(contentWidth, FooterHeight));

        Format line = new Format(contentWidth, 1)
        {
            Borders = new Border(new Shade(0.3, "#1565C0"), new Shade(), new Shade(), new Shade(), BorderStyle.dashed)
        };
        footer.AddColumn(SampleColumns.FooterLine, line);

        Format note = SampleFormats.CreateTextFormat(0, 3, 110, 5, 8, colour: "#424242");
        note.TextDecoration = TextDecoration.Underline;
        footer.AddColumn(SampleColumns.FooterNote, note);

        Format strike = SampleFormats.CreateTextFormat(0, 8, 110, 5, 8, italic: true, colour: "#757575");
        strike.TextDecoration = TextDecoration.Line;
        footer.AddColumn(SampleColumns.FooterStrike, strike);

        Format watermark = SampleFormats.CreateTextFormat((decimal)contentWidth - 95, 2, 45, 9, 14, TextAlignment.Center, 700, colour: "red");
        watermark.Angle = -10;
        watermark.Foreground = 5;
        watermark.FontDetails.ColorSize.Opacity = 0.35f;
        watermark.Borders = new Border(new Shade(0.4, "red", new Kernel(2)), BorderStyle.@double);
        footer.AddColumn(SampleColumns.FooterWatermark, watermark);

        decimal paginationLeft = (decimal)contentWidth - 40;
        footer.AddColumn(SampleColumns.FooterPageLabel, SampleFormats.CreateTextFormat(paginationLeft, 5, 14, 5, 9, TextAlignment.Right));
        footer.AddCurentPage(SampleFormats.CreateTextFormat(paginationLeft + 15, 5, 8, 5, 9, TextAlignment.Center));
        footer.AddColumn(SampleColumns.FooterOfLabel, SampleFormats.CreateTextFormat(paginationLeft + 23, 5, 6, 5, 9, TextAlignment.Center));
        footer.AddTotalPages(SampleFormats.CreateTextFormat(paginationLeft + 29, 5, 8, 5, 9, TextAlignment.Center));
    }
}
