using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleHeaderSetup
{
    public const double HeaderHeight = 25;

    public static void Configure(Section header, double contentWidth)
    {
        header.Format.Dimension = new Dimension(contentWidth, HeaderHeight);
        header.Row = new Row(new Dimension(contentWidth, HeaderHeight));

        header.AddColumn(SampleColumns.HeaderBackground,
            SampleFormats.CreateBoxFormat(0, 0, contentWidth, HeaderHeight, "#E3F2FD", BorderStyle.solid, 0.4, "#1565C0", 3));

        Format title = SampleFormats.CreateTextFormat(5, 3, 120, 8, 16, fontWeight: 700, colour: "#0D47A1");
        title.Foreground = 2;
        header.AddColumn(SampleColumns.HeaderTitle, title);

        Format subtitle = SampleFormats.CreateTextFormat(5, 12, 120, 5, 9, italic: true, colour: "gray");
        subtitle.Foreground = 2;
        header.AddColumn(SampleColumns.HeaderSubtitle, subtitle);

        header.AddColumn(SampleColumns.HeaderLogo, SampleFormats.CreateImageFormat((decimal)contentWidth - 62, 3, 30, 15));
        header.AddColumn(SampleColumns.HeaderBadge, SampleFormats.CreateImageFormat((decimal)contentWidth - 24, 4, 12, 12));

        decimal paginationLeft = 5;
        header.AddColumn(SampleColumns.HeaderPageLabel, SampleFormats.CreateTextFormat(paginationLeft, 18, 12, 5, 8));
        header.AddCurentPage(SampleFormats.CreateTextFormat(paginationLeft + 12, 18, 8, 5, 8, TextAlignment.Center));
        header.AddColumn(SampleColumns.HeaderOfLabel, SampleFormats.CreateTextFormat(paginationLeft + 20, 18, 6, 5, 8, TextAlignment.Center));
        header.AddTotalPages(SampleFormats.CreateTextFormat(paginationLeft + 26, 18, 8, 5, 8, TextAlignment.Center));
    }
}
