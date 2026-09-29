namespace DigitalDoor.Reporting.Presenters.PDF.Common;

internal static class MillimeterMath
{
    public const float PdfPointsPerInch = 72f;

    public static float MillimeterToPoint(double millimeters)
    {
        return MeasurementConverter.MillimetersToPixels(millimeters, PdfPointsPerInch);
    }

    public static float MillimeterToPoint(decimal millimeters)
    {
        return MeasurementConverter.MillimetersToPixels(millimeters, PdfPointsPerInch);
    }
}
