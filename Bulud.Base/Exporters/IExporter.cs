namespace Bulud.Base.Exporters;

public interface IExporter
{
    bool CanHandle(string acceptHeader);
    string ContentType { get; }
    string FileExtension { get; }
    byte[] Export<T>(IEnumerable<T> data);
}