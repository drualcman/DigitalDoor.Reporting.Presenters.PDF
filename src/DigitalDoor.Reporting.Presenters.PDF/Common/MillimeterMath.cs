namespace DigitalDoor.Reporting.Presenters.PDF.Common;
internal static class MillimeterMath
{
    const int DPI = 75;
    public static float MillimeterToPixel(double milimiter)
    {
        return MeasurementConverter.MillimetersToPixels(milimiter, DPI);
    }

    public static float MillimeterToPixel(decimal milimiter)
    {
        return MeasurementConverter.MillimetersToPixels(milimiter, DPI);
    }
}
