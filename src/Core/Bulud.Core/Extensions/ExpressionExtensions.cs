using System.Linq.Expressions;

namespace Bulud.Core.Extensions;

public static class ExpressionExtensions
{
    public static Expression BuildNestedProperty(Expression parameter, string propertyPath)
    {
        return propertyPath.Split('.').Aggregate(parameter, Expression.Property);
    }
}
