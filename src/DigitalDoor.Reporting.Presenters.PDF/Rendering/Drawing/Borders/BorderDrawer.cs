namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing.Borders;

internal static class BorderDrawer
{
    public static void Draw(SKCanvas canvas, SKRect box, Border borders)
    {
        BorderStyle style = ReportBorderLayout.GetEffectiveStyle(borders);
        if (style != BorderStyle.hidden)
        {
            float topWidth = CellGeometry.GetVisibleWidth(borders, borders.Top);
            if (topWidth > 0 && ReportBorderLayout.HasSameBorderOnAllSides(borders) && !BorderStyleBands.UsesSideTones(style))
            {
                UniformBorderDrawer.Draw(canvas, box, borders, style, topWidth, ColorTranslatorHelper.ConvertToSKColor(borders.Top.Colour));
            }
            else
            {
                DrawSide(canvas, box, borders, style, BorderSide.Top, borders.Top);
                DrawSide(canvas, box, borders, style, BorderSide.Right, borders.Right);
                DrawSide(canvas, box, borders, style, BorderSide.Bottom, borders.Bottom);
                DrawSide(canvas, box, borders, style, BorderSide.Left, borders.Left);
            }
        }
    }

    private static void DrawSide(SKCanvas canvas, SKRect box, Border borders, BorderStyle style, BorderSide side, Shade shade)
    {
        float borderWidth = CellGeometry.GetVisibleWidth(borders, shade);
        if (borderWidth > 0)
        {
            SideBorderDrawer.Draw(canvas, box, style, side, borderWidth, ColorTranslatorHelper.ConvertToSKColor(shade.Colour));
        }
    }
}
