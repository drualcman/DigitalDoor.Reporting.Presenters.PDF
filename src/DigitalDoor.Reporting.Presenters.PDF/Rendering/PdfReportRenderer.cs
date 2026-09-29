namespace DigitalDoor.Reporting.Presenters.PDF.Rendering;

internal static class PdfReportRenderer
{
    public static byte[] Render(ReportViewModel report, IReportFont reportFont)
    {
        if (report is null)
        {
            throw new ArgumentNullException(nameof(report));
        }
        using PdfRenderingContext context = new PdfRenderingContext(reportFont);
        ReportLayoutContent content = ReportLayoutContentBuilder.Build(report);
        FontSubsetPlanner.ReplaceUsedTypefacesWithSubsets(content, context.Typefaces);
        ReportPageLayout layout = ReportPageLayout.Create(report);
        List<ReportBodyPage> bodyPages = ReportBodyPaginator.Paginate(report.Body, content.BodyRows);
        ReportPageComposer pageComposer = new ReportPageComposer(report, layout, content, context);
        SKRect pageBox = CellGeometry.ToPoints(new LayoutBox(0, 0, layout.PageWidth, layout.PageHeight));
        return PdfDocumentWriter.Write(pageBox.Width, pageBox.Height, bodyPages.Count,
            (pageCanvas, pageIndex) => pageComposer.DrawPage(pageCanvas, bodyPages[pageIndex]));
    }
}
