namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing;

internal static class ImageCellDrawer
{
    private static readonly SKSamplingOptions HighQualitySampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

    public static void Draw(SKCanvas canvas, ReportImage image, SKRect cellBox, SKRect contentBox, Border borders)
    {
        if (contentBox.Width > 0 && contentBox.Height > 0)
        {
            int saveCount = canvas.Save();
            using SKRoundRect clipShape = CellGeometry.GetRoundedBox(cellBox, borders);
            canvas.ClipRoundRect(clipShape, SKClipOperation.Intersect, true);
            if (image.IsVector)
            {
                DrawVector(canvas, image, contentBox);
            }
            else
            {
                canvas.DrawImage(image.RasterImage, contentBox, HighQualitySampling);
            }
            canvas.RestoreToCount(saveCount);
        }
    }

    private static void DrawVector(SKCanvas canvas, ReportImage image, SKRect contentBox)
    {
        SKRect sourceBounds = image.GetSourceBounds();
        float scale = Math.Min(contentBox.Width / sourceBounds.Width, contentBox.Height / sourceBounds.Height);
        float offsetX = contentBox.Left + (contentBox.Width - sourceBounds.Width * scale) / 2f;
        float offsetY = contentBox.Top + (contentBox.Height - sourceBounds.Height * scale) / 2f;
        SKMatrix pictureMatrix = SKMatrix.CreateTranslation(-sourceBounds.Left, -sourceBounds.Top)
            .PostConcat(SKMatrix.CreateScale(scale, scale))
            .PostConcat(SKMatrix.CreateTranslation(offsetX, offsetY));
        canvas.DrawPicture(image.VectorPicture, in pictureMatrix);
    }
}
