## Create a PDF
To create a PDF need to manage the report ViewModel using a Handler and then use with the abtractions
``` csharp
    byte[] pdf = await IReportAsBytes.GenerateReport(reportModel);
```
IReportAsBytes it's registered by services.AddReportsServices();