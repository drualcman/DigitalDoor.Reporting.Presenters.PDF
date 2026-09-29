using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleReportSetupFactory
{
    public const decimal PageMargin = 10;

    public static Setup CreatePortraitSetup()
    {
        Setup setup = new Setup(PageSize.A4);
        ConfigurePage(setup, PageSize.A4.Width, PageSize.A4.Height, 2);
        return setup;
    }

    public static Setup CreateLandscapeSetup()
    {
        Setup setup = new Setup(PageSize.A4, Orientation.Landscape);
        ConfigurePage(setup, PageSize.A4.Height, PageSize.A4.Width, 3);
        return setup;
    }

    private static void ConfigurePage(Setup setup, double visiblePageWidth, double visiblePageHeight, int columnsNumber)
    {
        setup.Page.Margin = new Kernel(PageMargin);
        setup.Page.Background = "white";
        double contentWidth = visiblePageWidth - (double)(PageMargin * 2);
        double contentHeight = visiblePageHeight - (double)(PageMargin * 2);
        double bodyHeight = contentHeight - SampleHeaderSetup.HeaderHeight - SampleFooterSetup.FooterHeight;
        SampleHeaderSetup.Configure(setup.Header, contentWidth);
        SampleBodySetup.Configure(setup.Body, contentWidth, bodyHeight, columnsNumber);
        SampleFooterSetup.Configure(setup.Footer, contentWidth);
    }
}
