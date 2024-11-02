namespace DigitalDoor.Reporting.Presenters.PDF.Common;
public class MeasurementConverter
{
    private const float MILLIMETERS_PER_INCH = 25.4f;
    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(float mm, float dpi = 300)
    {
        if (dpi <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be greater than 0.");
        }
        return mm * dpi / MILLIMETERS_PER_INCH;
    }

    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(double mm, float dpi = 300) => MillimetersToPixels((float)mm, dpi);

    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(int mm, float dpi = 300) => MillimetersToPixels((float)mm, dpi);

    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(long mm, float dpi = 300) => MillimetersToPixels((float)mm, dpi);

    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(short mm, float dpi = 300) => MillimetersToPixels((float)mm, dpi);

    /// <summary>
    /// Convert milimeter to pixel depends of the DPI
    /// </summary>
    /// <param name="mm">Milimeters to convert.</param>
    /// <param name="dpi">Device DPI.</param>
    /// <returns>Pixels.</returns>
    public static float MillimetersToPixels(decimal mm, float dpi = 300) => MillimetersToPixels((float)mm, dpi);
}
