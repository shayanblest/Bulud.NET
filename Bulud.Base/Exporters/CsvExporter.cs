using System.Globalization;
using System.Text;
using CsvHelper;

namespace Bulud.Base.Exporters;

public class CsvExporter : IExporter
{
    public string ContentType => "text/csv";
    public string FileExtension => "csv";

    public bool CanHandle(string acceptHeader)
        => acceptHeader.Contains("text/csv", StringComparison.InvariantCultureIgnoreCase);

    public byte[] Export<T>(IEnumerable<T> data)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true));
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
        writer.Flush();
        return memoryStream.ToArray();
    }
}