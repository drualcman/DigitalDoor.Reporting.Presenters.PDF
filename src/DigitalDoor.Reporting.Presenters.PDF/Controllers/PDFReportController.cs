using DigitalDoor.Reporting.Entities.ViewModels;

namespace DigitalDoor.Reporting.Presenters.PDF.Controllers;
internal class PDFReportController : IReportAsBytes
{
    readonly IPDFReportOutputPort OutputPort;
    readonly IPDFReportPresenter Presenter;

    public PDFReportController(IPDFReportOutputPort outputPort, IPDFReportPresenter presenter)
    {
        OutputPort = outputPort;
        Presenter = presenter;
    }

    public async Task<byte[]> GenerateReport(ReportViewModel reportModel)
    {
        await OutputPort.Handle(reportModel);
        return Presenter.Report;
    }
}
