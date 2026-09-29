namespace DigitalDoor.Reporting.Presenters.PDF.Presenters;

internal class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort, IReportAsBytes<PDFReportPresenter>
{
    private readonly IReportFont ReportFont;

    public PDFReportPresenter(IReportFont reportFont)
    {
        ReportFont = reportFont;
    }

    public PDFReportPresenter() : this(null)
    {
    }

    public byte[] Report { get; private set; }

    public Task Handle(ReportViewModel report)
    {
        Report = PdfReportRenderer.Render(report, ReportFont);
        return Task.CompletedTask;
    }

    public Task<byte[]> GenerateReport(ReportViewModel report)
    {
        byte[] pdf = PdfReportRenderer.Render(report, ReportFont);
        Report = pdf;
        return Task.FromResult(pdf);
    }
}
