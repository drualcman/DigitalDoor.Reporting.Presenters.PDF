## Create a PDF
To create a PDF need to manage the report ViewModel using a Handler and then use with the abtractions
``` csharp
    class SomeClass
    {
        readonly IReportAsBytes ReportBytes;                //using default implementation
        public SomeClass(IReportAsBytes reportPdf)
        {
            ReportBytes = reportPdf;
        }

        public async Task GenerateReportAync(ReportViewModel reportModel)
        {
            PdfResponse response = new();
            byte[] report = await ReportBytes.GenerateReport(reportModel);
        }
    }

    class SomeClass
    {
        readonly IReportAsBytes<PDFReportPresenter> ReportBytes;                //using PDF implementation, recommended
        public SomeClass(IReportAsBytes<PDFReportPresenter> reportPdf)
        {
            ReportBytes = reportPdf;
        }

        public async Task GenerateReportAync(ReportViewModel reportModel)
        {
            PdfResponse response = new();
            byte[] report = await ReportBytes.GenerateReport(reportModel);
        }
    }
```
IReportAsBytes it's registered by services.AddReportingPresenterPdfServices();
IReportAsBytes< PDFReportPresenter > it's registered by services.AddReportingPresenterPdfServices(); but you can create your own.