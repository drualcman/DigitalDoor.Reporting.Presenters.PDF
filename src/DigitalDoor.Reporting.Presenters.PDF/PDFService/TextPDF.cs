using DigitalDoor.Reporting.Entities.Models;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class TextPDF
{
    readonly ReportViewModel ReportViewModel;
    readonly TextMapperParagraph MapperParagraph;
    readonly TextMapperImage MapperImage;
    readonly TextMapperBorder MapperBorder;
    readonly TextMapperBase MapperBase;
    readonly TextHelper Helper;
    readonly decimal HeightHeader;
    readonly decimal HeightFooter;
    readonly decimal HeightBody;
    readonly int TotalPages;

    public TextPDF(ReportViewModel reportViewModel)
    {
        ReportViewModel = reportViewModel;
        MapperParagraph = new TextMapperParagraph();
        MapperImage = new TextMapperImage();
        MapperBorder = new TextMapperBorder();
        MapperBase = new TextMapperBase();
        Helper = new TextHelper();
        HeightHeader = (decimal)(ReportViewModel.Header.Format.Dimension.Height + ReportViewModel.Body.Format.Dimension.Height + ReportViewModel.Footer.Format.Dimension.Height);
        HeightBody = (decimal)(ReportViewModel.Body.Format.Dimension.Height + ReportViewModel.Footer.Format.Dimension.Height);
        HeightFooter = (decimal)ReportViewModel.Footer.Format.Dimension.Height;
        TotalPages = reportViewModel.Pages;
    }

    public async Task<byte[]> CreatePDFReport()
    {
        using MemoryStream OutputStream = new MemoryStream();
        PdfWriter PdfWriter = new PdfWriter(OutputStream);
        PdfDocument PdfDocument = new PdfDocument(PdfWriter);
        PageSize Size = new PageSize(MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Dimension.Width),
            MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Dimension.Height));
        if (ReportViewModel.Page.Orientation == Report.Orientation.Landscape)
        {
            Size = Size.Rotate();
        }
        PdfDocument.SetDefaultPageSize(Size);
        await CreateDocument(PdfDocument);
        return OutputStream.ToArray();
    }

    private async Task CreateDocument(PdfDocument pdfDocument)
    {
        Document Document = new Document(pdfDocument);
        Document.SetMargins(MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Margin.Top), MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Margin.Right),
            MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Margin.Bottom), MillimeterMath.MillimeterToPixel(ReportViewModel.Page.Margin.Left));
        await SetContentDocument(Document);
        Document.Close();
    }

    private async Task SetContentDocument(Document document)
    {
        List<Task> elementsWorker = new List<Task>();
        List<ColumnContent> HeaderElements = default;
        List<ColumnContent> FooterElements = default;
        List<ColumnContent> BodyRows = default;
        elementsWorker.Add(Task.Run(() => HeaderElements = Helper.GetElements(ReportViewModel.Header.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Header).ToList())));
        elementsWorker.Add(Task.Run(() => FooterElements = Helper.GetElements(ReportViewModel.Footer.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Footer).ToList())));
        elementsWorker.Add(Task.Run(() => BodyRows = Helper.GetElements(ReportViewModel.Body.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Body).ToList())));
        await Task.WhenAll(elementsWorker);
        int RowsByPages = (int)(ReportViewModel.Body.Format.Dimension.Height / ReportViewModel.Body.Row.Dimension.Height);
        decimal HeightBodyElement = HeightBody;
        int ColumnsNumber = ReportViewModel.Body.ColumnsNumber;
        List<List<ColumnContent>> Pages = Helper.Split(BodyRows, RowsByPages);
        int PageNumber = 1;
        decimal ColumnWeight = 0;
        int page = 0;
        do
        {
            List<Task> columnWorker = new List<Task>();
            int CounterColumn = 0;
            if (page > 0)
            {
                PageNumber += 1;
                ColumnWeight = 0;
                HeightBodyElement = HeightBody;
            }
            columnWorker.Add(DrawBackground(document, PageNumber));
            columnWorker.Add(DrawHeader(document, HeaderElements, PageNumber));
            DrawBody(document, ColumnsNumber, Pages, ref PageNumber, ref ColumnWeight, ref page, columnWorker, ref CounterColumn);
            columnWorker.Add(DrawFooter(document, FooterElements, PageNumber));
            await Task.WhenAll(columnWorker);
            page++;
        } while (page < Pages.Count);
    }

    private Task DrawBackground(Document page, int PageNumber)
    {
        MapperBase.DrawBackground(page, ReportViewModel.Header.Format.Background, PageNumber, ReportViewModel.Header.Format.Dimension.Height, HeightBody);
        MapperBase.DrawBackground(page, ReportViewModel.Body.Format.Background, PageNumber, ReportViewModel.Body.Format.Dimension.Height, HeightFooter);
        MapperBase.DrawBackground(page, ReportViewModel.Footer.Format.Background, PageNumber, ReportViewModel.Footer.Format.Dimension.Height, 0);
        return Task.CompletedTask;
    }

    private async Task DrawHeader(Document page, List<ColumnContent> HeaderElements, int PageNumber)
    {
        if (HeaderElements != null)
        {
            ColumnContent CurrentPageHeader = HeaderElements[0]?.Columns.Where(d => d.Column.DataColumn.PropertyName == "CurrentPage").FirstOrDefault();
            if (CurrentPageHeader != null)
            {
                CurrentPageHeader.Value = PageNumber.ToString();
            }
            ColumnContent TotalPagesHeader = HeaderElements[0]?.Columns.Where(d => d.Column.DataColumn.PropertyName == "TotalPages").FirstOrDefault();
            if (CurrentPageHeader != null)
            {
                TotalPagesHeader.Value = TotalPages.ToString();
            }
            int header_lines = HeaderElements.Count;
            for (int h = 0; h < header_lines; h++)
            {
                await DrawContent(page, HeaderElements[h], HeightHeader, PageNumber, 0, HeightBody);
            }
        }
    }

    private void DrawBody(Document page, int ColumnsNumber, List<List<ColumnContent>> Pages, ref int PageNumber, ref decimal ColumnWeight, ref int i,
        List<Task> columnWorker, ref int CounterColumn)
    {
        if (Pages.Any())
        {
            columnWorker.Add(CreateBodyElements(page, Pages[i], HeightBody, PageNumber, ColumnWeight, HeightFooter));

            while (CanContinueNextColumn(ColumnsNumber, Pages.Count, i, CounterColumn))
            {
                CounterColumn += 1;
                i += 1;
                ColumnWeight += (decimal)(ReportViewModel.Body.Row.Dimension.Width + ReportViewModel.Body.ColumnsSpace);
                columnWorker.Add(CreateBodyElements(page, Pages[i], HeightBody, PageNumber, ColumnWeight, HeightFooter));
            }
        }
    }


    private async Task DrawFooter(Document page, List<ColumnContent> FooterElements, int PageNumber)
    {
        if (FooterElements != null)
        {
            ColumnContent CurrentPageFooter = FooterElements[0]?.Columns.Where(d => d.Column.DataColumn.PropertyName == "CurrentPage").FirstOrDefault();
            if (CurrentPageFooter != null)
            {
                CurrentPageFooter.Value = PageNumber.ToString();
            }
            ColumnContent TotalPagesFooter = FooterElements[0]?.Columns.Where(d => d.Column.DataColumn.PropertyName == "TotalPages").FirstOrDefault();
            if (CurrentPageFooter != null)
            {
                TotalPagesFooter.Value = TotalPages.ToString();
            }

            int footer_lines = FooterElements.Count;
            for (int f = 0; f < footer_lines; f++)
            {
                await DrawContent(page, FooterElements[f], HeightFooter, PageNumber, 0, 0);
            }
        }
    }

    private static bool CanContinueNextColumn(int columnsNumber, int pages, int i, int counterColumn) =>
        columnsNumber > counterColumn + 1 && i + 1 < pages;

    private async Task CreateBodyElements(Document page, List<ColumnContent> pagesElements, decimal heightBodyElement, int numberPage, decimal columnWeight, decimal heightBackground)
    {
        List<ColumnContent> PageElements = pagesElements;
        for (int r = 0; r < PageElements.Count; r++)
        {
            ColumnContent Element = PageElements[r];
            if (Element != null)
            {
                ColumnContent TotalPagesBody = Element.Columns.Where(d => d.Column.DataColumn.PropertyName == "TotalPages").FirstOrDefault();
                if (TotalPagesBody != null)
                {
                    TotalPagesBody.Value = PageElements.Count.ToString();
                }
                ColumnContent CurrentPagesBody = Element.Columns.Where(d => d.Column.DataColumn.PropertyName == "CurrentPage").FirstOrDefault();
                if (CurrentPagesBody != null)
                {
                    CurrentPagesBody.Value = numberPage.ToString();
                }
            }
            if (ReportViewModel.Body.Row.Borders != null)
            {
                ColumnSetup setup = new ColumnSetup();
                setup.Format.Borders = ReportViewModel.Body.Row.Borders;
                setup.Format.Dimension.Width = ReportViewModel.Body.Row.Dimension.Width;
                setup.Format.Dimension.Height = ReportViewModel.Body.Row.Dimension.Height;
                setup.Format.Position = ReportViewModel.Body.Format.Position;
                Div BorderBody = MapperBorder.SetBorder(setup, heightBodyElement - ReportViewModel.Body.Format.Position.Top, columnWeight);
                BorderBody.SetPageNumber(numberPage);
                page.Add(BorderBody);
            }
            await DrawContent(page, Element, heightBodyElement, numberPage, columnWeight + ReportViewModel.Body.Format.Position.Left, heightBackground);
            heightBodyElement -= (decimal)ReportViewModel.Body.Row.Dimension.Height;
        }
    }

    private Task DrawContent(Document page, ColumnContent format, decimal height, int PositionPage, decimal weight, decimal heightBackground)
    {
        format.Columns = format.Columns.OrderBy(d => d.Column.Format.Foreground).ToList();
        foreach (var item in format.Columns)
        {
            string Content = item.Value;
            if (Content != null)
            {
                if (string.IsNullOrWhiteSpace(Content))
                {
                    Div BorderSpaceWhite = MapperBorder.SetBorder(item.Column, height, weight);
                    BorderSpaceWhite.SetPageNumber(PositionPage);
                    page.Add(BorderSpaceWhite);
                    MapperBase.DrawBackground(page, item.Column.Format.Background, PositionPage, item.Column.Format.Dimension.Height, heightBackground);
                }
                else
                {
                    Paragraph Text = MapperParagraph.SetParagraph(Content, item, height, weight);
                    Text.SetPageNumber(PositionPage);
                    page.Add(Text);
                }
            }
            else
            {
                if (item.Image != null && item.Image.Length > 0)
                {
                    Image Image = MapperImage.SetImage(item.Image, item, height, weight);
                    Image.SetPageNumber(PositionPage);
                    page.Add(Image);
                    Div BorderSpaceWhite = MapperBorder.SetBorder(item.Column, height, weight);
                    BorderSpaceWhite.SetPageNumber(PositionPage);
                    page.Add(BorderSpaceWhite);
                }
            }
        }
        return Task.CompletedTask;
    }
}
