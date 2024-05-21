namespace DigitalDoor.Reporting.Presenters.Images.Common;
internal static class MillimeterMath
{
    private const double MILLIMETERS_PER_INCH = 25.4;
    public static int MillimeterToPixelInt(int millimeters, int dpi = 300) =>
        (int)Math.Round(MillimeterToPixelDouble((decimal)millimeters, dpi)); 
    public static int MillimeterToPixelInt(double millimeters, int dpi = 300) =>
        (int)Math.Round(MillimeterToPixelDouble(millimeters, dpi));
    public static int MillimeterToPixelInt(decimal millimeters, int dpi = 300) =>
        (int)Math.Round(MillimeterToPixelDecimal(millimeters, dpi)); 
    public static int MillimeterToPixelInt(float millimeters, int dpi = 300) =>
        (int)Math.Round(MillimeterToPixelFloat(millimeters, dpi));

    public static float MillimeterToPixelFloat(double millimeters, int dpi = 300) =>
        (float)MilimmeterToInches(millimeters) * dpi;
    public static float MillimeterToPixelFloat(decimal millimeters, int dpi = 300) =>
        (float)MilimmeterToInches(millimeters) * dpi; 
    public static float MillimeterToPixelFloat(float millimeters, int dpi = 300) =>
        MilimmeterToInches(millimeters) * dpi; 

    public static double MillimeterToPixelDouble(double millimeters, int dpi = 300) =>
        MilimmeterToInches(millimeters) * dpi;
    public static double MillimeterToPixelDouble(decimal millimeters, int dpi = 300) =>
        (double)MilimmeterToInches(millimeters) * dpi;  
    public static double MillimeterToPixelDouble(float millimeters, int dpi = 300) =>
        (double)MilimmeterToInches(millimeters) * dpi; 

    public static decimal MillimeterToPixelDecimal(double millimeters, int dpi = 300) =>
        (decimal)MilimmeterToInches(millimeters) * dpi;
    public static decimal MillimeterToPixelDecimal(decimal millimeters, int dpi = 300) =>
        MilimmeterToInches(millimeters) * dpi;
    public static decimal MillimeterToPixelDecimal(float millimeters, int dpi = 300) =>
        (decimal)MilimmeterToInches(millimeters) * dpi;

    private static double MilimmeterToInches(double millimeters) =>
        millimeters / MILLIMETERS_PER_INCH; 
    private static decimal MilimmeterToInches(decimal millimeters) =>
        millimeters / (decimal)MILLIMETERS_PER_INCH; 
    private static float MilimmeterToInches(float millimeters) =>
        millimeters / (float)MILLIMETERS_PER_INCH;
}
