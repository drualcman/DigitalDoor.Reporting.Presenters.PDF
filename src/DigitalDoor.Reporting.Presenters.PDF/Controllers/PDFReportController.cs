namespace DigitalDoor.Reporting.Presenters.PDF.Controllers;
internal class PDFReportController : IReportAsBytes
{
    readonly IReportAsBytes<PDFReportPresenter> Presenter;

    public PDFReportController(IReportAsBytes<PDFReportPresenter> presenter) =>
        Presenter = presenter;

    public async Task<byte[]> GenerateReport(ReportViewModel reportModel) =>
        await Presenter.GenerateReport(reportModel);
}
