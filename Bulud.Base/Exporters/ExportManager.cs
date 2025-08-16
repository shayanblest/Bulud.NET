namespace Bulud.Base.Exporters;

public class ExportManager(IEnumerable<IExporter> exporters)
{
    public (byte[] Data, string ContentType, string FileName)? TryExport<T>(
        IEnumerable<T> data,
        string? acceptHeader,
        string fileNameWithoutExtension)
    {
        var strategy = exporters.FirstOrDefault(s =>
            s.ContentType.Equals(acceptHeader, StringComparison.OrdinalIgnoreCase));

        if (strategy == null)
            return null;

        var bytes = strategy.Export(data);
        var fileName = fileNameWithoutExtension + strategy.FileExtension;
        return (bytes, strategy.ContentType, fileName);
    }
}