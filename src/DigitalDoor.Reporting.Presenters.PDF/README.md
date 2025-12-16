## Create a PDF
To create a PDF need to manage the report ViewModel using a Handler and then use with the abtractions
``` csharp
    class SomeClass
    {
        readonly IReportAsBytes ReportBytes;                //using default implementation, recommended
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
        readonly IReportAsBytes<PDFReportPresenter> ReportBytes;                //using own PDF implementation
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
`IReportAsBytes` it's registered by `services.AddReportingServices()` and `services.AddReportingPresenterImageServices()`
`IReportAsBytes<PDFReportPresenter>` it's registered by `services.AddReportingPresenterPdfServices()` but you can create your own.


# Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

- Fork the repository
- Create your feature branch (git checkout -b feature/AmazingFeature)
- Commit your changes (git commit -m 'Add some AmazingFeature')
- Push to the branch (git push origin feature/AmazingFeature)
- Open a Pull Request

# License
This project is licensed under the GNU AFFERO GENERAL PUBLIC LICENSE.

# Acknowledgments
- Built with love for the Blazor community
- Inspired by the need for simple, effective dashboard components
- Thanks to all contributors and users

# Made with love by DrUalcman

If you find this component useful, please consider giving it a star on GitHub!