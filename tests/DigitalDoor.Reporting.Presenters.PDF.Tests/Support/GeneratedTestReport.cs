namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public sealed class GeneratedTestReport
{
    public byte[] Pdf { get; }
    public ReportViewModel ViewModel { get; }

    public GeneratedTestReport(byte[] pdf, ReportViewModel viewModel)
    {
        Pdf = pdf;
        ViewModel = viewModel;
    }
}
