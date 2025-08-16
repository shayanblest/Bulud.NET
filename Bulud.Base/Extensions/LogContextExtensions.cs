using Serilog.Context;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Bulud.Base.Extensions;

public static class LogContextExtensions
{
    public static Dictionary<string, object?> CollectLogMetadata(this HttpContext context, Stopwatch stopwatch)
    {
        return new()
        {
            { "path", context.Request.Path.Value },
            { "status", context.Response.StatusCode },
            { "user", context.User?.Identity?.Name ?? "Anonymous" },
            { "method", context.Request.Method },
            { "request_id", context.TraceIdentifier },
            { "user_agent", context.Request.Headers["User-Agent"].ToString() },
            { "client_ip", context.Connection.RemoteIpAddress?.ToString() },
            { "query", context.Request.QueryString.ToString() },
            { "duration_ms", stopwatch.ElapsedMilliseconds }
        };
    }

    public static IDisposable PushToLogContext(this Dictionary<string, object?> logData)
    {
        var disposables = logData
            .Select(kv => LogContext.PushProperty(kv.Key, kv.Value))
            .ToList();

        return new CompositeDisposable(disposables);
    }

    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public CompositeDisposable(List<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var d in _disposables)
                d.Dispose();
        }
    }
}