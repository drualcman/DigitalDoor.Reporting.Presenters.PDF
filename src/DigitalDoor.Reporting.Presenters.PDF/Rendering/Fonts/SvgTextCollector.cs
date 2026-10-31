using System.Xml;
using System.Xml.Linq;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class SvgTextCollector
{
    private static readonly HashSet<string> TextElementNames = new(StringComparer.Ordinal) { "text", "tspan", "textPath" };

    public static List<SvgTextRun> Collect(string svgMarkup)
    {
        List<SvgTextRun> textRuns = new List<SvgTextRun>();
        XDocument document = ParseOrNull(svgMarkup);
        if (document?.Root is not null)
        {
            CollectFromElement(document.Root, SvgFontState.Default, textRuns);
        }
        return textRuns;
    }

    private static void CollectFromElement(XElement element, SvgFontState inheritedState, List<SvgTextRun> textRuns)
    {
        SvgFontState state = inheritedState.Inherit(element);
        if (TextElementNames.Contains(element.Name.LocalName))
        {
            string text = string.Concat(element.Nodes().OfType<XText>().Select(textNode => textNode.Value));
            if (!string.IsNullOrWhiteSpace(text))
            {
                textRuns.Add(new SvgTextRun(state.ToFont(), text));
            }
        }
        foreach (XElement childElement in element.Elements())
        {
            CollectFromElement(childElement, state, textRuns);
        }
    }

    private static XDocument ParseOrNull(string svgMarkup)
    {
        XDocument document;
        XmlReaderSettings readerSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
        try
        {
            using StringReader markupReader = new StringReader(svgMarkup ?? string.Empty);
            using XmlReader xmlReader = XmlReader.Create(markupReader, readerSettings);
            document = XDocument.Load(xmlReader);
        }
        catch (XmlException)
        {
            document = null;
        }
        return document;
    }
}
