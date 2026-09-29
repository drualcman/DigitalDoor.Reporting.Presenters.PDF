namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public static class TestSetups
{
    public const string BodyText = "BodyText";
    public const string HeaderText = "HeaderText";
    public const string FooterText = "FooterText";

    public static Setup CreatePaginatedSetup(double bodyHeight = 100, double rowHeight = 10, int columnsNumber = 1)
    {
        Setup setup = new Setup(PageSize.A4);
        setup.Page.Margin = new Kernel(10);
        setup.Header.Format.Dimension = new Dimension(190, 20);
        setup.Header.AddColumn(HeaderText, CreateTextFormat(0, 0, 100, 8));
        setup.Header.AddCurentPage(CreateTextFormat(120, 0, 10, 8));
        setup.Header.AddTotalPages(CreateTextFormat(140, 0, 10, 8));
        setup.Body.Format.Dimension = new Dimension(190, bodyHeight);
        setup.Body.ColumnsNumber = columnsNumber;
        setup.Body.Row = new Row(new Dimension(90, rowHeight));
        setup.Body.AddColumn(BodyText, CreateTextFormat(0, 0, 90, rowHeight));
        setup.Footer.Format.Dimension = new Dimension(190, 20);
        setup.Footer.AddColumn(FooterText, CreateTextFormat(0, 0, 100, 8));
        setup.Footer.AddCurentPage(CreateTextFormat(120, 0, 10, 8));
        setup.Footer.AddTotalPages(CreateTextFormat(140, 0, 10, 8));
        return setup;
    }

    public static Format CreateTextFormat(decimal left, decimal top, double width, double height, string fontName = "Arial", int size = 10)
    {
        return new Format(width, height)
        {
            Position = new Kernel(top, left),
            TextAlignment = TextAlignment.Left,
            FontDetails = new Font(fontName, size, "black")
        };
    }

    public static List<ColumnData> CreateData(int bodyRows, string headerText = "HEADER", string footerText = "FOOTER")
    {
        List<ColumnData> data = new List<ColumnData>
        {
            new ColumnData(new Item(HeaderText), headerText, SectionType.Header),
            new ColumnData(new Item(FooterText), footerText, SectionType.Footer)
        };
        for (int rowNumber = 1; rowNumber <= bodyRows; rowNumber++)
        {
            data.Add(new ColumnData(new Item(BodyText), $"Row{rowNumber}", SectionType.Body) { Row = rowNumber });
        }
        return data;
    }
}
