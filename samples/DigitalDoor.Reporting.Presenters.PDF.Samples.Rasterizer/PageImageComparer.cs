using SkiaSharp;

public static class PageImageComparer
{
    private const int ChannelTolerance = 60;

    public static double Compare(string firstImagePath, string secondImagePath, string differenceImagePath)
    {
        using SKBitmap firstImage = SKBitmap.Decode(firstImagePath);
        using SKBitmap secondImage = SKBitmap.Decode(secondImagePath);
        int width = Math.Min(firstImage.Width, secondImage.Width);
        int height = Math.Min(firstImage.Height, secondImage.Height);
        using SKBitmap differenceImage = new SKBitmap(width, height);
        long differentPixels = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SKColor firstColor = firstImage.GetPixel(x, y);
                SKColor secondColor = secondImage.GetPixel(x, y);
                bool isDifferent = Math.Abs(firstColor.Red - secondColor.Red) > ChannelTolerance ||
                    Math.Abs(firstColor.Green - secondColor.Green) > ChannelTolerance ||
                    Math.Abs(firstColor.Blue - secondColor.Blue) > ChannelTolerance;
                byte faded = (byte)(200 + (firstColor.Red + firstColor.Green + firstColor.Blue) / 3 * 55 / 255);
                differenceImage.SetPixel(x, y, isDifferent ? SKColors.Red : new SKColor(faded, faded, faded));
                differentPixels += isDifferent ? 1 : 0;
            }
        }
        using (FileStream differenceStream = File.Create(differenceImagePath))
        {
            differenceImage.Encode(differenceStream, SKEncodedImageFormat.Png, 100);
        }
        return differentPixels * 100d / (width * (double)height);
    }
}
