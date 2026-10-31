namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Document;

internal sealed class ReportPageComposer
{
    private readonly ReportViewModel Report;
    private readonly ReportPageLayout Layout;
    private readonly ReportLayoutContent Content;
    private readonly CellDrawer CellDrawer;
    private readonly BodyColumnsComposer BodyComposer;

    public ReportPageComposer(ReportViewModel report, ReportPageLayout layout, ReportLayoutContent content, PdfRenderingContext context)
    {
        Report = report;
        Layout = layout;
        Content = content;
        CellDrawer = new CellDrawer(context.Typefaces, new ReportImageResolver(context));
        BodyComposer = new BodyColumnsComposer(report.Body, layout.Body, CellDrawer);
    }

    public void DrawPage(SKCanvas canvas, ReportBodyPage page)
    {
        BackgroundDrawer.Draw(canvas, CellGeometry.ToPoints(new LayoutBox(0, 0, Layout.PageWidth, Layout.PageHeight)), Report.Page?.Background, null);
        DrawSectionBox(canvas, Report.Header, Layout.Header);
        DrawSectionBox(canvas, Report.Body, Layout.Body);
        DrawSectionBox(canvas, Report.Footer, Layout.Footer);
        DrawSectionRowBorder(canvas, Report.Header, Layout.Header, Content.HeaderHasRows);
        CellDrawer.DrawCells(canvas, Content.HeaderCells, Layout.Header.ContentLeft, Layout.Header.ContentTop, page);
        BodyComposer.Draw(canvas, page);
        CellDrawer.DrawCells(canvas, Content.BodyPaginationCells, Layout.Body.ContentLeft, Layout.Body.ContentTop, page);
        DrawSectionRowBorder(canvas, Report.Footer, Layout.Footer, Content.FooterHasRows);
        CellDrawer.DrawCells(canvas, Content.FooterCells, Layout.Footer.ContentLeft, Layout.Footer.ContentTop, page);
    }

    private static void DrawSectionBox(SKCanvas canvas, Section section, ReportSectionLayout sectionLayout)
    {
        SKRect sectionBox = CellGeometry.ToPoints(sectionLayout.Bounds);
        BackgroundDrawer.Draw(canvas, sectionBox, section?.Format?.Background, section?.Format?.Borders);
        BorderDrawer.Draw(canvas, sectionBox, section?.Format?.Borders);
    }

    private static void DrawSectionRowBorder(SKCanvas canvas, Section section, ReportSectionLayout sectionLayout, bool sectionHasRows)
    {
        if (sectionHasRows && section?.Row?.Dimension is not null)
        {
            LayoutBox rowBox = new LayoutBox(sectionLayout.ContentLeft, sectionLayout.ContentTop, section.Row.Dimension.Width, section.Row.Dimension.Height);
            BorderDrawer.Draw(canvas, CellGeometry.ToPoints(rowBox), section.Row.Borders);
        }
    }
}
