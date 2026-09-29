using System.Globalization;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleHtmlDocument
{
    public static string Wrap(string title, string reportHtml, ReportViewModel reportViewModel)
    {
        bool isLandscape = reportViewModel.Page.Orientation == Orientation.Landscape;
        double pageWidth = isLandscape ? reportViewModel.Page.Dimension.Height : reportViewModel.Page.Dimension.Width;
        double pageHeight = isLandscape ? reportViewModel.Page.Dimension.Width : reportViewModel.Page.Dimension.Height;
        string pageSize = $"{pageWidth.ToString(CultureInfo.InvariantCulture)}mm {pageHeight.ToString(CultureInfo.InvariantCulture)}mm";
        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" />" +
            $"<title>{System.Net.WebUtility.HtmlEncode(title)}</title>" +
            "<style>body{margin:0;padding:10mm 0;background:#e0e0e0;}" +
            ".dd-report-page{margin:0 auto 10mm auto;box-shadow:0 0 2mm rgba(0,0,0,.25);}" +
            $"@media print{{@page{{size:{pageSize};margin:0;}}body{{padding:0;background:none;}}.dd-report-page{{margin:0;box-shadow:none;}}}}</style>" +
            $"</head><body>{reportHtml}</body></html>";
    }
}
