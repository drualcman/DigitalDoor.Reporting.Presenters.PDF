using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public static class PdfInspector
{
    public static List<string> GetPageTexts(byte[] pdf)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        return document.GetPages().Select(page => string.Join(" ", page.GetWords().Select(word => word.Text))).ToList();
    }

    public static List<Letter> GetLetters(byte[] pdf, int pageNumber)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        return document.GetPage(pageNumber).Letters.ToList();
    }

    public static (double Width, double Height) GetPageSize(byte[] pdf, int pageNumber)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        Page page = document.GetPage(pageNumber);
        return (page.Width, page.Height);
    }

    public static HashSet<string> GetFontNames(byte[] pdf)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        return document.GetPages().SelectMany(page => page.Letters).Select(letter => letter.FontName).ToHashSet();
    }

    public static int CountImages(byte[] pdf)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        return document.GetPages().Sum(page => page.GetImages().Count());
    }

    public static int GetPageCount(byte[] pdf)
    {
        using PdfDocument document = PdfDocument.Open(pdf);
        return document.NumberOfPages;
    }
}
