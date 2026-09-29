namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing;

internal sealed class CellDrawer
{
    private readonly ReportTypefaceProvider Typefaces;
    private readonly ReportImageResolver Images;

    public CellDrawer(ReportTypefaceProvider typefaces, ReportImageResolver images)
    {
        Typefaces = typefaces;
        Images = images;
    }

    public void DrawCells(SKCanvas canvas, IEnumerable<ReportLayoutCell> cells, double originLeft, double originTop, ReportBodyPage page)
    {
        foreach (ReportLayoutCell cell in cells)
        {
            Draw(canvas, cell, originLeft, originTop, page);
        }
    }

    private void Draw(SKCanvas canvas, ReportLayoutCell cell, double originLeft, double originTop, ReportBodyPage page)
    {
        Format format = cell.Format;
        LayoutBox cellLayoutBox = ReportCellLayout.GetCellBox(format, originLeft, originTop);
        SKRect cellBox = CellGeometry.ToPoints(cellLayoutBox);
        SKRect paddingBox = CellGeometry.ToPoints(ReportCellLayout.GetPaddingBox(cellLayoutBox, format));
        SKRect contentBox = CellGeometry.ToPoints(ReportCellLayout.GetContentBox(cellLayoutBox, format));
        int saveCount = canvas.Save();
        canvas.RotateDegrees((float)format.Angle, cellBox.MidX, cellBox.MidY);
        BeginOpacityLayer(canvas, cellBox, format);
        BackgroundDrawer.Draw(canvas, cellBox, format.Background, format.Borders);
        ReportImage image = Images.ResolveOrNull(cell);
        if (image is not null)
        {
            ImageCellDrawer.Draw(canvas, image, paddingBox, contentBox, format.Borders);
        }
        else
        {
            DrawText(canvas, cell.GetTextForPage(page.PageNumber, page.TotalPages), paddingBox, contentBox, format);
        }
        BorderDrawer.Draw(canvas, cellBox, format.Borders);
        canvas.RestoreToCount(saveCount);
    }

    private void DrawText(SKCanvas canvas, string text, SKRect paddingBox, SKRect contentBox, Format format)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            TextCellDrawer.Draw(canvas, text, paddingBox, contentBox, format, Typefaces);
        }
    }

    private static void BeginOpacityLayer(SKCanvas canvas, SKRect cellBox, Format format)
    {
        float opacity = ReportCellLayout.GetOpacity(format);
        if (opacity < 1f)
        {
            using SKPaint layerPaint = new SKPaint { Color = SKColors.Black.WithAlpha(ColorTranslatorHelper.ConvertOpacityToAlpha(opacity)) };
            canvas.SaveLayer(cellBox, layerPaint);
        }
    }
}
