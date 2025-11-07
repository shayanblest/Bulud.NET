using System.ComponentModel.DataAnnotations;

namespace Bulud.Base.Extensions;

public static class ValidationHelper
{
    public static List<ValidationResult> Validate<T>(T dto)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto, null, null);
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return results;
    }
}