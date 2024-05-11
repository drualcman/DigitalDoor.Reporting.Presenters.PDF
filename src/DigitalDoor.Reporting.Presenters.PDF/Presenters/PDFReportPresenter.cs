namespace DigitalDoor.Reporting.Presenters.PDF.Presenters;

internal class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort, IReportAsBytes<PDFReportPresenter>
{

    public PDFReportPresenter(IReportFont reportFont)
    {
        FontService.GetReportFont(reportFont);
    }
    public PDFReportPresenter()
    {

    }

    public byte[] Report { get; private set; }

    public async Task Handle(ReportViewModel report)
    {
        TextPDF PDF = new(report);
        Report = await PDF.CreatePDFReport();
        FontService.DisposeFont();
    }

    public async Task<byte[]> GenerateReport(ReportViewModel report)
    {
        await Handle(report);
        return Report;
    }
}
