namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Content;

internal sealed class ReportImageResolver
{
    private readonly PdfRenderingContext Context;

    public ReportImageResolver(PdfRenderingContext context)
    {
        Context = context;
    }

    public ReportImage ResolveOrNull(ReportLayoutCell cell)
    {
        ReportImage image = null;
        ReportCellValue value = cell.Value;
        if (cell.Pagination == ReportCellPagination.None && value is not null)
        {
            Func<ReportImage> createImage = GetImageFactory(value, cell.ColumnDescription);
            image = createImage is null ? null : Context.GetOrCreateImage(value.SourceValue, createImage);
        }
        return image;
    }

    private static Func<ReportImage> GetImageFactory(ReportCellValue value, string columnDescription)
    {
        Func<ReportImage> createImage = value.Kind switch
        {
            ReportCellValueKind.RasterImage => () => ReportImageDecoder.DecodeRaster(value.RasterImage, columnDescription),
            ReportCellValueKind.SvgImage => () => ReportImageDecoder.DecodeSvg(value.Svg, columnDescription),
            ReportCellValueKind.Other => GetDrawingObjectFactory(value.SourceValue),
            _ => null
        };
        return createImage;
    }

    private static Func<ReportImage> GetDrawingObjectFactory(object drawingObject)
    {
        Func<ReportImage> createImage = drawingObject switch
        {
            SKImage rasterImage => () => ReportImage.FromBorrowedRaster(rasterImage),
            SKBitmap bitmap => () => ReportImageDecoder.FromBitmap(bitmap),
            SKPicture picture => () => ReportImage.FromBorrowedVector(picture),
            _ => null
        };
        return createImage;
    }
}
