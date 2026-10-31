using Svg.Skia;
using Svg.Skia.TypefaceProviders;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Images;

internal static class ReportImageDecoder
{
    public static ReportImage DecodeRaster(byte[] encodedImage, string columnDescription)
    {
        SKImage rasterImage = SKImage.FromEncodedData(encodedImage) ??
            throw new InvalidOperationException($"The image of the column '{columnDescription}' could not be decoded. Supported formats are PNG, JPEG, GIF, BMP, WEBP, ICO and SVG.");
        return ReportImage.FromOwnedRaster(rasterImage);
    }

    public static ReportImage DecodeSvg(string svgMarkup, string columnDescription, ITypefaceProvider typefaceProvider)
    {
        SKSvg svgDocument = new SKSvg();
        svgDocument.Settings.TypefaceProviders = new List<ITypefaceProvider> { typefaceProvider };
        SKPicture picture = LoadSvgPicture(svgDocument, svgMarkup);
        if (picture is null || picture.CullRect.Width <= 0 || picture.CullRect.Height <= 0)
        {
            svgDocument.Dispose();
            throw new InvalidOperationException($"The SVG image of the column '{columnDescription}' could not be parsed or has no size.");
        }
        return ReportImage.FromOwnedVector(picture, svgDocument);
    }

    public static ReportImage FromBitmap(SKBitmap bitmap)
    {
        return ReportImage.FromOwnedRaster(SKImage.FromBitmap(bitmap));
    }

    private static SKPicture LoadSvgPicture(SKSvg svgDocument, string svgMarkup)
    {
        SKPicture picture;
        try
        {
            picture = svgDocument.FromSvg(svgMarkup);
        }
        catch (Exception exception)
        {
            svgDocument.Dispose();
            throw new InvalidOperationException("The SVG image could not be parsed.", exception);
        }
        return picture;
    }
}
