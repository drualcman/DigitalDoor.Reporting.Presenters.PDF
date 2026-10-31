using System.Xml.Linq;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal sealed class SvgFontState
{
    private const int BoldWeight = 700;
    private const int NormalWeight = 400;

    private SvgFontState(string families, int weight, bool italic)
    {
        Families = families;
        Weight = weight;
        Italic = italic;
    }

    public static SvgFontState Default { get; } = new SvgFontState(null, NormalWeight, false);

    public string Families { get; }
    public int Weight { get; }
    public bool Italic { get; }

    public SvgFontState Inherit(XElement element)
    {
        Dictionary<string, string> declarations = ReadDeclarations(element);
        string families = declarations.TryGetValue("font-family", out string declaredFamilies) ? declaredFamilies : Families;
        int weight = declarations.TryGetValue("font-weight", out string declaredWeight) ? ReadWeight(declaredWeight) : Weight;
        bool italic = declarations.TryGetValue("font-style", out string declaredStyle) ?
            declaredStyle.Trim().Equals("italic", StringComparison.OrdinalIgnoreCase) ||
            declaredStyle.Trim().Equals("oblique", StringComparison.OrdinalIgnoreCase) : Italic;
        return new SvgFontState(families, weight, italic);
    }

    public Font ToFont()
    {
        return SvgFontFamilyName.CreateFont(Families, FontStyleHelper.IsBoldWeight(Weight), Italic);
    }

    private int ReadWeight(string declaredWeight)
    {
        string weightText = declaredWeight.Trim();
        int result = int.TryParse(weightText, out int numericWeight) ? numericWeight : Weight;
        if (weightText.Equals("bold", StringComparison.OrdinalIgnoreCase) || weightText.Equals("bolder", StringComparison.OrdinalIgnoreCase))
        {
            result = BoldWeight;
        }
        else if (weightText.Equals("normal", StringComparison.OrdinalIgnoreCase) || weightText.Equals("lighter", StringComparison.OrdinalIgnoreCase))
        {
            result = NormalWeight;
        }
        return result;
    }

    private static Dictionary<string, string> ReadDeclarations(XElement element)
    {
        Dictionary<string, string> declarations = new(StringComparer.OrdinalIgnoreCase);
        foreach (XAttribute attribute in element.Attributes().Where(attribute => attribute.Name.NamespaceName.Length == 0))
        {
            declarations[attribute.Name.LocalName] = attribute.Value;
        }
        string style = (string)element.Attribute("style") ?? string.Empty;
        foreach (string declaration in style.Split(';'))
        {
            int separatorIndex = declaration.IndexOf(':');
            if (separatorIndex > 0)
            {
                declarations[declaration.Substring(0, separatorIndex).Trim()] = declaration.Substring(separatorIndex + 1).Trim();
            }
        }
        return declarations;
    }
}
