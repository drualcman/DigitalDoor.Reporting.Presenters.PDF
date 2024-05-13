namespace DigitalDoor.Reporting.Presenters.PDF.Common;
internal static class MillimeterMath
{
    public static float MillimeterToPixel(double milimiter)
    {
        return (float)(milimiter * 2.83);
    }

    public static float MillimeterToPixel(decimal milimiter)
    {
        return (float)((double)milimiter * 2.83);
    }
}
