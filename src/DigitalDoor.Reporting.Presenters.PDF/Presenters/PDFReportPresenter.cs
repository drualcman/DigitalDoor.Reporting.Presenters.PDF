using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.Interfaces;
using DigitalDoor.Reporting.Presenters.PDF.PDFService;
using DigitalDoor.Reporting.Presenters.PDF.Utilities;

namespace DigitalDoor.Reporting.Presenters.PDF.Presenters;

internal class PDFReportPresenter : IPDFReportPresenter, IPDFReportOutputPort
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
}
