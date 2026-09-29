namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Drawing;

internal static class CellGeometry
{
    public static SKRect ToPoints(LayoutBox box)
    {
        return SKRect.Create(
            MillimeterMath.MillimeterToPoint(box.Left),
            MillimeterMath.MillimeterToPoint(box.Top),
            MillimeterMath.MillimeterToPoint(Math.Max(0, box.Width)),
            MillimeterMath.MillimeterToPoint(Math.Max(0, box.Height)));
    }

    public static SKRoundRect GetRoundedBox(SKRect box, Border borders)
    {
        double[] radiiInMillimeters = ReportBorderLayout.GetCornerRadii(borders);
        SKPoint[] radii = radiiInMillimeters
            .Select(radius => new SKPoint(MillimeterMath.MillimeterToPoint(radius), MillimeterMath.MillimeterToPoint(radius)))
            .ToArray();
        SKRoundRect roundedBox = new SKRoundRect();
        roundedBox.SetRectRadii(box, radii);
        return roundedBox;
    }

    public static float GetVisibleWidth(Border borders, Shade side)
    {
        return MillimeterMath.MillimeterToPoint(ReportBorderLayout.GetVisibleWidth(borders, side));
    }
}
