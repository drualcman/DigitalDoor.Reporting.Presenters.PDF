using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.Presenters.Images.Common;
using SkiaSharp;
using System.Text.Json;

namespace DigitalDoor.Reporting.Presenters.Images;

public class JPGPresenter : IReportAsBytes<JPGPresenter>
{
    const int DPI = 300;
    public Task<byte[]> GenerateReport(ReportViewModel reportModel)
    {
        int width = (int)MeasurementConverter.MillimetersToPixels(reportModel.Page.Dimension.Width, DPI);
        int height = (int)MeasurementConverter.MillimetersToPixels(reportModel.Page.Dimension.Height, DPI);

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(ColorTranslatorHelper.ConvertToSKColor(reportModel.Page.Background.ToLower() == "transparent" ? "white" : reportModel.Page.Background.ToLower()));

        DrawItems(canvas, reportModel.Header.Items, reportModel);
        DrawItems(canvas, reportModel.Body.Items, reportModel);
        DrawItems(canvas, reportModel.Footer.Items, reportModel);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return Task.FromResult(data.ToArray());
    }

    private void DrawItems(SKCanvas canvas, List<ColumnSetup> items, ReportViewModel reportModel)
    {
        foreach (var item in items)
        {
            var data = reportModel.GetColumnData(item.DataColumn);
            var format = item.Format;
            float xPos = MeasurementConverter.MillimetersToPixels(format.Position.Left, DPI);
            float yPos = MeasurementConverter.MillimetersToPixels(format.Position.Top, DPI);

            if (ImageValidator.IsLikelyImage(data.Value.ToString()))
            {
                var jsonValue = (JsonElement)data.Value;
                if (jsonValue.TryGetBytesFromBase64(out var image))
                {
                    AddBytesToCanvas(canvas, image, xPos, yPos, format.Dimension, format.Borders);
                }
            }
            else if (data?.Value is byte[] imageBytes)
            {
                AddBytesToCanvas(canvas, imageBytes, xPos, yPos, format.Dimension, format.Borders);
            }
            else
            {
                string text = data.Value.ToString();
                DrawTextWithRoundedBackground(canvas, text, format, xPos, yPos, format.Dimension);
            }
        }
    }

    private void AddBytesToCanvas(SKCanvas canvas, byte[] imageBytes, float xPos, float yPos, Dimension dimensions, Border border)
    {
        using var ms = new MemoryStream(imageBytes);
        using var skImage = SKBitmap.Decode(ms);

        float itemWidth = MeasurementConverter.MillimetersToPixels(dimensions.Width, DPI);
        float itemHeight = MeasurementConverter.MillimetersToPixels(dimensions.Height, DPI);

        var destRect = new SKRect(xPos, yPos, xPos + itemWidth, yPos + itemHeight);
        canvas.DrawBitmap(skImage, destRect);
        ApplyBorderStyle(canvas, destRect, border);
    }

    private void DrawTextWithRoundedBackground(SKCanvas canvas, string text, Format format, float xPos, float yPos, Dimension dimensions)
    {      
        float textSizePx = MeasurementConverter.MillimetersToPixels(format.FontDetails.ColorSize.Width, DPI / 4);

        SKPaint p = new SKPaint();

        var paint = new SKPaint
        {
            TextSize = textSizePx,
            Color = ColorTranslatorHelper.ConvertToSKColor(format.FontDetails.ColorSize.Colour),
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(format.FontDetails.FontName, FontStyleHelper.ConvertToSKFontStyle(format.FontDetails.FontStyle))
        };

        float itemWidth = MeasurementConverter.MillimetersToPixels(dimensions.Width, DPI);
        float itemHeight = MeasurementConverter.MillimetersToPixels(dimensions.Height, DPI);
        var outerRect = new SKRect(xPos, yPos, xPos + itemWidth, yPos + itemHeight);

        // Fondo redondeado
        using var backgroundPaint = new SKPaint { Color = ColorTranslatorHelper.ConvertToSKColor(format.Background) };
        canvas.DrawRoundRect(outerRect, (float)format.Borders.Top.Radius.Left, (float)format.Borders.Top.Radius.Bottom, backgroundPaint);

        // Dibujar el borde
        DrawBorders(canvas, outerRect, format.Borders);

        // Texto centrado en el rectángulo
        canvas.DrawText(text, xPos, yPos + textSizePx, paint);
    }

    private void DrawBorders(SKCanvas canvas, SKRect rect, Border borders)
    {
        float leftX = rect.Left;
        float topY = rect.Top;
        float rightX = rect.Right;
        float bottomY = rect.Bottom;

        using (var topPaint = new SKPaint
        {
            Color = ColorTranslatorHelper.ConvertToSKColor(borders.Top.Colour),
            StrokeWidth = (float)borders.Top.Width,
            Style = SKPaintStyle.Stroke
        })
        {
            canvas.DrawLine(leftX, topY, rightX, topY, topPaint);
        }

        using (var bottomPaint = new SKPaint
        {
            Color = ColorTranslatorHelper.ConvertToSKColor(borders.Bottom.Colour),
            StrokeWidth = (float)borders.Bottom.Width,
            Style = SKPaintStyle.Stroke
        })
        {
            canvas.DrawLine(leftX, bottomY, rightX, bottomY, bottomPaint);
        }

        using (var leftPaint = new SKPaint
        {
            Color = ColorTranslatorHelper.ConvertToSKColor(borders.Left.Colour),
            StrokeWidth = (float)borders.Left.Width,
            Style = SKPaintStyle.Stroke
        })
        {
            canvas.DrawLine(leftX, topY, leftX, bottomY, leftPaint);
        }

        using (var rightPaint = new SKPaint
        {
            Color = ColorTranslatorHelper.ConvertToSKColor(borders.Right.Colour),
            StrokeWidth = (float)borders.Right.Width,
            Style = SKPaintStyle.Stroke
        })
        {
            canvas.DrawLine(rightX, topY, rightX, bottomY, rightPaint);
        }
    }


    private void ApplyBorderStyle(SKCanvas canvas, SKRect bounds, Border border)
    {
        var borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = ColorTranslatorHelper.ConvertToSKColor(border.Top.Colour),
            StrokeWidth = (float)border.Top.Width,
            PathEffect = border.Style switch
            {
                BorderStyle.dotted => SKPathEffect.CreateDash(new float[] { 1, 2 }, 0),
                BorderStyle.dashed => SKPathEffect.CreateDash(new float[] { 4, 4 }, 0),
                _ => null,
            }
        };

        canvas.DrawRoundRect(bounds, (float)border.Top.Radius.Top, (float)border.Top.Radius.Top, borderPaint);
    }
}
