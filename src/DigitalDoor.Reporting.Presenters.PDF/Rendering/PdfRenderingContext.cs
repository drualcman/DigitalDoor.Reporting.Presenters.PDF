namespace DigitalDoor.Reporting.Presenters.PDF.Rendering;

internal sealed class PdfRenderingContext : IDisposable
{
    private readonly Dictionary<object, ReportImage> ImagesBySourceValue = new(ReferenceEqualityComparer.Instance);
    private readonly List<ReportImage> ImagesWithoutCache = new();

    public ReportTypefaceProvider Typefaces { get; }
    public ReportSvgTypefaceProvider SvgTypefaces { get; }

    public PdfRenderingContext(IReportFont reportFont)
    {
        Typefaces = new ReportTypefaceProvider(reportFont);
        SvgTypefaces = new ReportSvgTypefaceProvider(Typefaces);
    }

    public ReportImage GetOrCreateImage(object sourceValue, Func<ReportImage> createImage)
    {
        ReportImage image;
        if (sourceValue is null || sourceValue.GetType().IsValueType)
        {
            image = createImage();
            ImagesWithoutCache.Add(image);
        }
        else if (!ImagesBySourceValue.TryGetValue(sourceValue, out image))
        {
            image = createImage();
            ImagesBySourceValue[sourceValue] = image;
        }
        return image;
    }

    public void Dispose()
    {
        foreach (ReportImage image in ImagesBySourceValue.Values.Concat(ImagesWithoutCache))
        {
            image.Dispose();
        }
        ImagesBySourceValue.Clear();
        ImagesWithoutCache.Clear();
        Typefaces.Dispose();
    }
}
