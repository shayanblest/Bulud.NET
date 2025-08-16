using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bulud.Base.Exceptions;

public class AppValidationException(Dictionary<string, string[]> errors, string? title = null) : Exception
{
    public ValidationProblemDetails ProblemDetails { get; } = new(errors)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? "برخی از اطلاعات وارد شده نادرست هستند. لطفاً خطاهای زیر را بررسی کنید."
            : title,
        Status = StatusCodes.Status400BadRequest,
    };
}