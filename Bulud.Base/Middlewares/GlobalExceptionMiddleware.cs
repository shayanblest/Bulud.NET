﻿using System.Diagnostics;
using System.Security.Authentication;
using System.Text.Json;
using Bulud.Base.Exceptions;
using Bulud.Base.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bulud.Base.Middlewares;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment env,
    IHttpContextAccessor httpContextAccessor)
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
            stopwatch.Stop();

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden &&
                !context.Response.HasStarted)
            {
                throw new ForbiddenException();
            }

            LogInfo(context, stopwatch);
        }
        catch (AppValidationException ex) // TODO: Refactor Global Exception handler (use catch for every type of exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.ProblemDetails, _options));
        }
        catch (ForbiddenException ex)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var path = context.Request.Path.Value;
            var status = context.Response.StatusCode;
            var user = httpContextAccessor.HttpContext?.User.GetUserName() ?? "Anonymous";
            var method = context.Request.Method;
            var queryString = context.Request.QueryString.ToString();
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            logger.LogWarning(
                "{Exception} => {StatusCode} | {Method} {Path}{Query} | User: {User} | IP: {IP} | UA: {UserAgent}",
                "Forbidden", status, method, path, queryString, user, clientIp, userAgent);
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status403Forbidden,
                Instance = context.Request.Path
            });
        }
        catch (InvalidCredentialException ex)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            logger.LogWarning("{Exception} => {UserName} | {Ip}", "InvalidCredentials", ex.Message, clientIp);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = "اطلاعات هویتی ارائه نشده است.",
                Status = StatusCodes.Status401Unauthorized,
            });
        }
        catch (NotFoundException ex)
        {
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status404NotFound,
            });
        }
        catch (DuplicateRequestException ex)
        {
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status409Conflict,
            });
        }
        catch (BadHttpRequestException ex)
        {
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
            });
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateException dbEx &&
                dbEx.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true)
            {
                var fieldName = ExtractFieldFromSqlException(dbEx.InnerException.Message) ?? "UnknownField";
                var errors = new Dictionary<string, string[]>
                {
                    { fieldName, ["Duplicated"] }
                };
                await WriteProblemResponseAsync(context, new ValidationProblemDetails(errors)
                {
                    Title = "یک یا چند خطا در اعتبارسنجی داده\u200cها رخ داده است.",
                    Status = StatusCodes.Status409Conflict,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                });
                return;
            }

            var traceId = Activity.Current != null
                ? $"00-{Activity.Current.TraceId}-{Activity.Current.SpanId}-00"
                : context.TraceIdentifier;

            LogError(context, ex, traceId);
            await WriteProblemResponseAsync(context, new ProblemDetails
            {
                Title = "خطای غیرمنتظره\u200cای رخ داده است.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = env.IsDevelopment() ? ex.ToString() : null,
                Extensions = { ["traceId"] = traceId }
            });
        }
    }

    private async Task WriteProblemResponseAsync(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, _options));
    }

    private async Task WriteProblemResponseAsync(HttpContext context, ValidationProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, _options));
    }

    private string? ExtractFieldFromSqlException(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        const string marker = "with unique index '";
        var startIndex = message.IndexOf(marker, StringComparison.Ordinal);
        if (startIndex == -1)
            return null;

        startIndex += marker.Length;
        var endIndex = message.IndexOf("'", startIndex, StringComparison.Ordinal);
        if (endIndex == -1)
            return null;

        var indexName = message[startIndex..endIndex];

        var parts = indexName.Split('_');
        if (parts.Length > 0)
            return parts.Last();

        return null;
    }

    private void LogInfo(HttpContext context, Stopwatch stopwatch)
    {
        var path = context.Request.Path.Value;
        var status = context.Response.StatusCode;
        var user = httpContextAccessor.HttpContext?.User.GetUserName() ?? "Anonymous";
        var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        var method = context.Request.Method;
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var queryString = context.Request.QueryString.ToString();
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
        logger.LogInformation(
            "Request ended => {Method} {Path}{Query} | Status: {StatusCode} | Elapsed: {Elapsed:0.000} ms | User: {User} | IP: {IP}",
            method, path, queryString, status, elapsedMs, user, clientIp);
    }

    private void LogError(HttpContext context, Exception ex, string traceId)
    {
        var path = context.Request.Path.Value;
        var user = httpContextAccessor.HttpContext?.User.GetUserName() ?? "Anonymous";
        var method = context.Request.Method;
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        var queryString = context.Request.QueryString.ToString();
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();

        logger.LogError(ex,
            "Unhandled Exception => {StatusCode} | {Method} {Path}{Query} | User: {User} | IP: {IP} | UA: {UserAgent} | {TraceId}",
            StatusCodes.Status500InternalServerError,
            method, path, queryString, user, clientIp, userAgent, traceId);
    }
}