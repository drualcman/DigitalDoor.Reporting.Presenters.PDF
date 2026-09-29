namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Document;

internal static class PdfDocumentWriter
{
    private const string DocumentCreator = "DigitalDoor.Reporting";
    private const string DocumentProducer = "DigitalDoor.Reporting.Presenters.PDF (SkiaSharp)";
    private const float RasterizationDotsPerInch = 300f;

    public static byte[] Write(float pageWidth, float pageHeight, int pageCount, Action<SKCanvas, int> drawPage)
    {
        using MemoryStream pdfStream = new MemoryStream();
        SKDocumentPdfMetadata metadata = SKDocumentPdfMetadata.Default;
        metadata.Creator = DocumentCreator;
        metadata.Producer = DocumentProducer;
        metadata.RasterDpi = RasterizationDotsPerInch;
        metadata.Creation = DateTime.Now;
        metadata.Modified = DateTime.Now;
        using (SKDocument document = SKDocument.CreatePdf(pdfStream, metadata) ??
            throw new InvalidOperationException("SkiaSharp could not create the PDF document. Check that the SkiaSharp native assets are available for this platform."))
        {
            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                SKCanvas pageCanvas = document.BeginPage(pageWidth, pageHeight);
                drawPage(pageCanvas, pageIndex);
                document.EndPage();
            }
            document.Close();
        }
        return pdfStream.ToArray();
    }
}
