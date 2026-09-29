namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Images;

internal sealed class ReportImage : IDisposable
{
    private readonly IDisposable OwnedResource;

    public SKImage RasterImage { get; }
    public SKPicture VectorPicture { get; }
    public bool IsVector => VectorPicture is not null;

    private ReportImage(SKImage rasterImage, SKPicture vectorPicture, IDisposable ownedResource)
    {
        RasterImage = rasterImage;
        VectorPicture = vectorPicture;
        OwnedResource = ownedResource;
    }

    public static ReportImage FromOwnedRaster(SKImage rasterImage) => new ReportImage(rasterImage, null, rasterImage);

    public static ReportImage FromBorrowedRaster(SKImage rasterImage) => new ReportImage(rasterImage, null, null);

    public static ReportImage FromOwnedVector(SKPicture vectorPicture, IDisposable pictureOwner) =>
        new ReportImage(null, vectorPicture, pictureOwner);

    public static ReportImage FromBorrowedVector(SKPicture vectorPicture) => new ReportImage(null, vectorPicture, null);

    public SKRect GetSourceBounds()
    {
        SKRect sourceBounds = IsVector ? VectorPicture.CullRect : new SKRect(0, 0, RasterImage.Width, RasterImage.Height);
        return sourceBounds;
    }

    public void Dispose()
    {
        OwnedResource?.Dispose();
    }
}
