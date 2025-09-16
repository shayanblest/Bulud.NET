using System.Security.Cryptography;
using Bulud.Base.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Bulud.Otp;

public class InMemoryOtpService(IMemoryCache cache, IOptions<OtpSettings> settings) : IOtpService
{
    public Task<string> GenerateAsync(string identityKey)
    {
        var code = GenerateRandomCode(settings.Value.CodeLength);

        cache.Set(
            key: GetCacheKey(identityKey),
            value: code,
            absoluteExpiration: DateTimeOffset.Now.AddMinutes(settings.Value.ExpireMinutes)
        );

        return Task.FromResult(code);
    }

    public Task<bool> VerifyAsync(string identityKey, string otpCode)
    {
        var cacheKey = GetCacheKey(identityKey);
        if (!cache.TryGetValue(cacheKey, out string? storedCode)) return Task.FromResult(false);
        if (storedCode != otpCode) return Task.FromResult(false);
        cache.Remove(cacheKey);
        return Task.FromResult(true);
    }

    private static string GetCacheKey(string identityKey) => $"OTP:{identityKey}";

    private static string GenerateRandomCode(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var sb = new System.Text.StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b % 10);
        }

        return sb.ToString();
    }
}

public class OtpSettings
{
    public int CodeLength { get; set; } = 6;
    public int ExpireMinutes { get; set; } = 2;
}