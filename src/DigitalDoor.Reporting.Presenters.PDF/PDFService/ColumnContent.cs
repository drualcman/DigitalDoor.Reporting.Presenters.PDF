using DigitalDoor.Reporting.Entities.Models;

namespace DigitalDoor.Reporting.Presenters.PDF.PDFService;

internal class ColumnContent
{
    public string Value { get; set; }
    public byte[] Image { get; set; }
    public ColumnSetup Column { get; set; }
    public List<ColumnContent> Columns { get; set; }
}
