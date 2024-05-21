using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.Presenters.Images.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;

namespace DigitalDoor.Reporting.Presenters.Images;

public class JPGPresenter : IReportAsBytes<JPGPresenter>
{
    public Task<byte[]> GenerateReport(ReportViewModel reportModel)
    {
        int dpi = 300;
        int width = MillimeterMath.MillimeterToPixelInt(reportModel.Page.Dimension.Width, dpi);
        int height = MillimeterMath.MillimeterToPixelInt(reportModel.Page.Dimension.Height, dpi);

        Bitmap bitmap = new Bitmap(width, height);
        bitmap.SetResolution(dpi, dpi);
        if (reportModel.Page.Orientation == Entities.ValueObjects.Orientation.Landscape)
        {
            bitmap = new Bitmap(height, width);
        }
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.CompositingQuality = CompositingQuality.HighQuality;


        graphics.Clear(ColorTranslator.FromHtml(reportModel.Page.Background.ToLower() == "transparent" ? "white" : reportModel.Page.Background.ToLower()));

        List<ColumnSetup> items = reportModel.Header.Items;
        int index = 0;
        bool shouldContinue = index < items.Count;

        while (shouldContinue)
        {
            ColumnSetup item = items[index];
            ColumnData data = reportModel.GetColumnData(item.DataColumn);
            Format format = item.Format;
            int xPos = MillimeterMath.MillimeterToPixelInt(format.Position.Left);
            int yPos = MillimeterMath.MillimeterToPixelInt(format.Position.Top);

            if (ImageValidator.IsLikelyImage(data.Value.ToString()))
            {
                JsonElement JsonValue = (JsonElement)data.Value;
                JsonValue.TryGetBytesFromBase64(out byte[] image);
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);

            }
            else if (data?.Value.GetType() == typeof(byte[]))
            {
                byte[] image = (byte[])data.Value;
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);
            }
            else
            {
                string text = data.Value.ToString();
                DrawTextWithRoundedBackground(graphics, text, format, xPos, yPos, format.Dimension);
            }
            index++;

            // Actualizar la condición para el siguiente ciclo
            shouldContinue = index < items.Count;
        }
        items = reportModel.Body.Items;
        index = 0;
        shouldContinue = index < items.Count;

        while (shouldContinue)
        {
            ColumnSetup item = items[index];
            ColumnData data = reportModel.GetColumnData(item.DataColumn);
            Format format = item.Format;
            int xPos = MillimeterMath.MillimeterToPixelInt(format.Position.Left);
            int yPos = MillimeterMath.MillimeterToPixelInt(format.Position.Top);

            if (ImageValidator.IsLikelyImage(data.Value.ToString()))
            {
                JsonElement JsonValue = (JsonElement)data.Value;
                JsonValue.TryGetBytesFromBase64(out byte[] image);
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);

            }
            else if (data?.Value.GetType() == typeof(byte[]))
            {
                byte[] image = (byte[])data.Value;
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);
            }
            else
            {
                string text = data.Value.ToString();
                DrawTextWithRoundedBackground(graphics, text, format, xPos, yPos, format.Dimension);
            }
            index++;

            // Actualizar la condición para el siguiente ciclo
            shouldContinue = index < items.Count;
        }

        items = reportModel.Footer.Items;
        index = 0;
        shouldContinue = index < items.Count;

        while (shouldContinue)
        {
            ColumnSetup item = items[index];
            ColumnData data = reportModel.GetColumnData(item.DataColumn);
            Format format = item.Format;
            int xPos = MillimeterMath.MillimeterToPixelInt(format.Position.Left);
            int yPos = MillimeterMath.MillimeterToPixelInt(format.Position.Top);

            if (ImageValidator.IsLikelyImage(data.Value.ToString()))
            {
                JsonElement JsonValue = (JsonElement)data.Value;
                JsonValue.TryGetBytesFromBase64(out byte[] image);
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);

            }
            else if (data?.Value.GetType() == typeof(byte[]))
            {
                byte[] image = (byte[])data.Value;
                AddBytesToBitmap(image, graphics, xPos, yPos, format.Dimension, format.Borders);
            }
            else
            {
                string text = data.Value.ToString();
                DrawTextWithRoundedBackground(graphics, text, format, xPos, yPos, format.Dimension);
            }
            index++;

            // Actualizar la condición para el siguiente ciclo
            shouldContinue = index < items.Count;
        }

        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Jpeg);
        bitmap.Dispose();
        return Task.FromResult(memoryStream.ToArray());
    }

    private Font GetFont(Format format) =>
        new Font(format.FontDetails.FontName, (float)format.FontDetails.ColorSize.Width, GetFontStyle(format));

    private FontStyle GetFontStyle(Format format)
    {
        Entities.ValueObjects.Font font = format.FontDetails;
        FontStyle style = FontStyle.Regular;
        if (font.FontStyle.IsBold)
            style |= FontStyle.Bold;
        if (font.FontStyle.Italic)
            style |= FontStyle.Italic;
        if (format.TextDecoration == Entities.ValueObjects.TextDecoration.Underline)
            style |= FontStyle.Underline;
        if (format.TextDecoration == Entities.ValueObjects.TextDecoration.Line)
            style |= FontStyle.Strikeout;
        return style;
    }

    private void AddBytesToBitmap(byte[] imageBytes, Graphics graphics, int xPos, int yPos, Entities.ValueObjects.Dimension dimensions, Entities.ValueObjects.Border border)
    {
        using MemoryStream ms = new MemoryStream(imageBytes);
        using Bitmap bitmap = new Bitmap(ms);

        // Ajustar las dimensiones para incluir el ancho del borde más grueso
        int maxBorderWidth = Math.Max(Math.Max(Convert.ToInt32(border.Top.Width), Convert.ToInt32(border.Bottom.Width)),
                                      Math.Max(Convert.ToInt32(border.Left.Width), Convert.ToInt32(border.Right.Width)));

        int itemHeight = MillimeterMath.MillimeterToPixelInt(dimensions.Height);
        int itemWidth = MillimeterMath.MillimeterToPixelInt(dimensions.Width);

        // Aumentar las dimensiones para asegurarse de que el borde no corte la imagen
        Rectangle bounds = new Rectangle(xPos, yPos, itemWidth + maxBorderWidth, itemHeight + maxBorderWidth);

        GraphicsPath path = GetPath(border, xPos, yPos, dimensions);

        // Establecer el área de recorte
        graphics.SetClip(path);
        // Asegurarse de dibujar la imagen en el área original, no en la extendida
        graphics.DrawImage(bitmap, new Rectangle(xPos + maxBorderWidth / 2, yPos + maxBorderWidth / 2, itemWidth, itemHeight));

        ApplyBorderStyle(graphics, bounds, border);
        graphics.ResetClip();  // Restablecer el área de recorte;

    }

    private GraphicsPath GetPath(Entities.ValueObjects.Border border, int xPos, int yPos, Entities.ValueObjects.Dimension dimensions)
    {
        GraphicsPath path = new GraphicsPath();
        Rectangle bounds = new Rectangle(xPos, yPos, MillimeterMath.MillimeterToPixelInt(dimensions.Width), MillimeterMath.MillimeterToPixelInt(dimensions.Height));

        // Si el borde tiene curvatura pero no grosor, considerar un grosor mínimo para propósitos de recorte
        float minimalWidth = 0.1f;  // Grosor mínimo para asegurar la visualización de la curvatura
        AddArcOrLine(path, bounds, border.Top.Radius.Top, Math.Max(border.Top.Width, minimalWidth), "top");
        AddArcOrLine(path, bounds, border.Right.Radius.Right, Math.Max(border.Right.Width, minimalWidth), "right");
        AddArcOrLine(path, bounds, border.Bottom.Radius.Bottom, Math.Max(border.Bottom.Width, minimalWidth), "bottom");
        AddArcOrLine(path, bounds, border.Left.Radius.Left, Math.Max(border.Left.Width, minimalWidth), "left");

        path.CloseFigure();  // Asegura que el camino esté cerrado para formar la figura
        return path;
    }

    private void AddArcOrLine(GraphicsPath path, Rectangle bounds, decimal radius, double width, string side)
    {
        int diameter = 2 * MillimeterMath.MillimeterToPixelInt(radius);
        if (width > 0)  // Solo añadir si el grosor es mayor a cero
        {
            switch (side)
            {
                case "top":
                    if (radius > 0) path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
                    else path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    break;
                case "right":
                    if (radius > 0) path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
                    else path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                    break;
                case "bottom":
                    if (radius > 0) path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                    else path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                    break;
                case "left":
                    if (radius > 0) path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                    else path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
                    break;
            }
        }
    }

    private void DrawTextWithRoundedBackground(Graphics graphics, string text, Format format, int xPos, int yPos, Entities.ValueObjects.Dimension dimensions)
    {
        Font font = GetFont(format);
        Entities.ValueObjects.Border border = format.Borders;
        Color textColor = ColorTranslatorHelper.GetColor(format.FontDetails.ColorSize.Colour);
        Color backgroundColor = ColorTranslatorHelper.GetColor(format.Background);
        Color borderColor = ColorTranslatorHelper.GetColor(format.Background);

        using GraphicsPath path = GetPath(border, xPos, yPos, dimensions);
        Rectangle bounds = new Rectangle(xPos, yPos, MillimeterMath.MillimeterToPixelInt(format.Dimension.Width), MillimeterMath.MillimeterToPixelInt(format.Dimension.Height));

        // Rellenar el fondo con el color especificado
        using SolidBrush brush = new SolidBrush(backgroundColor);
        graphics.FillPath(brush, path);

        // Dibujar el borde del rectángulo
        using Pen pen = new Pen(borderColor);
        graphics.DrawPath(pen, path);

        // Dibujar el texto
        StringFormat stringFormat = new StringFormat()
        {
            Alignment = GetStringAlignment(format.TextAlignment),
            LineAlignment = GetStringAlignment(format.TextAlignment)
        };

        ApplyBorderStyle(graphics, border, bounds);

        // Calcular el área de texto para centrar el texto en el rectángulo
        RectangleF textArea = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height);

        using SolidBrush textBrush = new SolidBrush(textColor);
        graphics.DrawString(text, font, textBrush, textArea, stringFormat);
    }

    private void ApplyBorderStyle(Graphics graphics, Entities.ValueObjects.Border border, Rectangle bounds)
    {
        // Utilizar sub-paths para aplicar estilos de borde específicos
        GraphicsPath topPath = new GraphicsPath();
        AddArcOrLine(topPath, bounds, border.Top.Radius.Top, border.Top.Width, "top");
        using Pen penTop = new Pen(ColorTranslatorHelper.GetColor(border.Top.Colour), MillimeterMath.MillimeterToPixelFloat(border.Top.Width))
        {
            DashStyle = GetDashStyle(border.Style)
        };
        graphics.DrawPath(penTop, topPath);
        topPath.CloseFigure();  // Asegura que el camino esté cerrado para formar la figura

        GraphicsPath rightPath = new GraphicsPath();
        AddArcOrLine(rightPath, bounds, border.Right.Radius.Top, border.Right.Width, "right");
        using Pen penRight = new Pen(ColorTranslatorHelper.GetColor(border.Right.Colour), MillimeterMath.MillimeterToPixelFloat(border.Right.Width))
        {
            DashStyle = GetDashStyle(border.Style)
        };
        graphics.DrawPath(penRight, rightPath);
        rightPath.CloseFigure();  // Asegura que el camino esté cerrado para formar la figura

        GraphicsPath bottomPath = new GraphicsPath();
        AddArcOrLine(bottomPath, bounds, border.Bottom.Radius.Top, border.Bottom.Width, "bottom");
        using Pen penBottom = new Pen(ColorTranslatorHelper.GetColor(border.Bottom.Colour), MillimeterMath.MillimeterToPixelFloat(border.Bottom.Width))
        {
            DashStyle = GetDashStyle(border.Style)
        };
        graphics.DrawPath(penBottom, bottomPath);
        bottomPath.CloseFigure();  // Asegura que el camino esté cerrado para formar la figura

        GraphicsPath leftPath = new GraphicsPath();
        AddArcOrLine(leftPath, bounds, border.Left.Radius.Top, border.Left.Width, "left");
        using Pen penLeft = new Pen(ColorTranslatorHelper.GetColor(border.Left.Colour), MillimeterMath.MillimeterToPixelFloat(border.Left.Width))
        {
            DashStyle = GetDashStyle(border.Style)
        };
        graphics.DrawPath(penLeft, leftPath);
        leftPath.CloseFigure();  // Asegura que el camino esté cerrado para formar la figura

    }


    private void ApplyBorderStyle(Graphics graphics, Rectangle bounds, Entities.ValueObjects.Border border)
    {
        // Aplicar el estilo de borde según las propiedades de `Border`
        if (border.Top.Width > 0)
        {
            using Pen penTop = new Pen(ColorTranslatorHelper.GetColor(border.Top.Colour), MillimeterMath.MillimeterToPixelFloat(border.Top.Width));

            penTop.DashStyle = GetDashStyle(border.Style);
            graphics.DrawLine(penTop, new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));

        }

        if (border.Right.Width > 0)
        {
            using Pen penRight = new Pen(ColorTranslatorHelper.GetColor(border.Right.Colour), MillimeterMath.MillimeterToPixelFloat(border.Right.Width));

            penRight.DashStyle = GetDashStyle(border.Style);
            graphics.DrawLine(penRight, new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom));

        }

        if (border.Bottom.Width > 0)
        {
            using Pen penBottom = new Pen(ColorTranslatorHelper.GetColor(border.Bottom.Colour), MillimeterMath.MillimeterToPixelFloat(border.Bottom.Width));

            penBottom.DashStyle = GetDashStyle(border.Style);
            graphics.DrawLine(penBottom, new Point(bounds.Right, bounds.Bottom), new Point(bounds.Left, bounds.Bottom));
        }


        if (border.Left.Width > 0)
        {
            using Pen penLeft = new Pen(ColorTranslatorHelper.GetColor(border.Left.Colour), MillimeterMath.MillimeterToPixelFloat(border.Left.Width));

            penLeft.DashStyle = GetDashStyle(border.Style);
            graphics.DrawLine(penLeft, new Point(bounds.Left, bounds.Bottom), new Point(bounds.Left, bounds.Top));

        }
    }

    private DashStyle GetDashStyle(Entities.ValueObjects.BorderStyle style)
    {
        return style switch
        {
            Entities.ValueObjects.BorderStyle.dotted => DashStyle.Dot,
            Entities.ValueObjects.BorderStyle.dashed => DashStyle.Dash,
            Entities.ValueObjects.BorderStyle.solid => DashStyle.Solid,
            _ => DashStyle.Solid,
        };
    }

    private StringAlignment GetStringAlignment(Entities.ValueObjects.TextAlignment alignment)
    {
        return alignment switch
        {
            Entities.ValueObjects.TextAlignment.Right => StringAlignment.Near,
            Entities.ValueObjects.TextAlignment.Left => StringAlignment.Far,
            _ => StringAlignment.Center,
        };
    }
}
