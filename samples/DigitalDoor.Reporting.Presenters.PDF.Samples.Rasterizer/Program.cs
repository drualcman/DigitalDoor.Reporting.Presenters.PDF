using PDFtoImage;
using SkiaSharp;

if (args[0] == "diff")
{
    double differentPercentage = PageImageComparer.Compare(args[1], args[2], args[3]);
    Console.WriteLine($"{args[3]}: {differentPercentage:0.###}% different pixels");
}
else if (args[0] == "crop")
{
    using SKBitmap sourceImage = SKBitmap.Decode(args[1]);
    SKRectI cropArea = new SKRectI(int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[3]) + int.Parse(args[5]), int.Parse(args[4]) + int.Parse(args[6]));
    using SKBitmap croppedImage = new SKBitmap(cropArea.Width, cropArea.Height);
    sourceImage.ExtractSubset(croppedImage, cropArea);
    using FileStream croppedStream = File.Create(args[2]);
    croppedImage.Encode(croppedStream, SKEncodedImageFormat.Png, 100);
}
else
{
    string outputDirectory = args[0];
    int dotsPerInch = int.Parse(args[1]);
    Directory.CreateDirectory(outputDirectory);
    foreach (string pdfPath in args.Skip(2))
    {
        byte[] pdfBytes = await File.ReadAllBytesAsync(pdfPath);
        string pdfName = Path.GetFileNameWithoutExtension(pdfPath);
        int pageIndex = 0;
        RenderOptions renderOptions = new RenderOptions(Dpi: dotsPerInch, BackgroundColor: SKColors.White);
        foreach (SKBitmap pageBitmap in Conversion.ToImages(pdfBytes, options: renderOptions))
        {
            pageIndex++;
            string pngPath = Path.Combine(outputDirectory, $"{pdfName}-page{pageIndex}.png");
            using (pageBitmap)
            using (FileStream pngStream = File.Create(pngPath))
            {
                pageBitmap.Encode(pngStream, SKEncodedImageFormat.Png, 100);
                Console.WriteLine($"{pngPath} ({pageBitmap.Width}x{pageBitmap.Height})");
            }
        }
    }
}
