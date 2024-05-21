namespace DigitalDoor.Reporting.Presenters.Images.Common;
internal static class ColorTranslatorHelper
{
    public static System.Drawing.Color GetColor(string color)
    {
        System.Drawing.Color drawColor;
        if (color.Contains('#'))
        {
            drawColor = System.Drawing.ColorTranslator.FromHtml(color);
        }
        else
        {
            drawColor = System.Drawing.Color.FromName(color);
        }
        return drawColor;
    }
}
