namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Document;

internal sealed class BodyColumnsComposer
{
    private readonly Section Body;
    private readonly ReportSectionLayout BodyLayout;
    private readonly CellDrawer CellDrawer;

    public BodyColumnsComposer(Section body, ReportSectionLayout bodyLayout, CellDrawer cellDrawer)
    {
        Body = body;
        BodyLayout = bodyLayout;
        CellDrawer = cellDrawer;
    }

    public void Draw(SKCanvas canvas, ReportBodyPage page)
    {
        Dimension rowDimension = ReportBodyPaginator.GetRowDimension(Body);
        double columnStep = ReportBodyPaginator.GetColumnStep(Body);
        for (int columnIndex = 0; columnIndex < page.Columns.Count; columnIndex++)
        {
            IReadOnlyList<ReportLayoutRow> columnRows = page.Columns[columnIndex];
            for (int rowIndex = 0; rowIndex < columnRows.Count; rowIndex++)
            {
                double rowLeft = BodyLayout.ContentLeft + columnIndex * columnStep;
                double rowTop = BodyLayout.ContentTop + rowIndex * rowDimension.Height;
                LayoutBox rowBox = new LayoutBox(rowLeft, rowTop, rowDimension.Width, rowDimension.Height);
                BorderDrawer.Draw(canvas, CellGeometry.ToPoints(rowBox), Body?.Row?.Borders);
                CellDrawer.DrawCells(canvas, columnRows[rowIndex].Cells, rowLeft, rowTop, page);
            }
        }
    }
}
