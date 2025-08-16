using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Bulud.Base.Queries;
using Microsoft.EntityFrameworkCore;

namespace Bulud.Base.Infrastructure
{
    public static class QueryableExtensions
    {
        private static readonly Regex FilterRegex =
            new Regex(@"(?:\s*(?<property>[\w\.]+):(?<operation>\w+)\((?<values>[^)]*)\))", RegexOptions.Compiled);

        private static readonly Regex SortRegex =
            new Regex(@"^(?<property>\w+)\((?<direction>asc|desc)\)$", RegexOptions.IgnoreCase);

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression)) return query;
            // Match the expression
            var match = SortRegex.Match(sortExpression);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid sort expression format. Expected 'property(direction)'.");
            }

            // Extract property and direction
            var propertyName = match.Groups["property"].Value;
            var direction = match.Groups["direction"].Value.ToLower();

            // Get the entity type
            var entityType = typeof(T);

            // Use reflection to find the property
            var propertyInfo = entityType.GetProperty(propertyName,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{entityType.Name}'.");
            }

            // Create the lambda expression for the property
            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            // Choose the correct method for sorting
            var methodName = direction == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            // Invoke the sorting method
            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, string? filter)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var matches = FilterRegex.Matches(filter);
            var parameter = Expression.Parameter(typeof(T), "x"); // Represents the entity in the query
            Expression? combinedExpression = null;

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var property = match.Groups["property"].Value;
                var operation = match.Groups["operation"].Value;
                var values = match.Groups["values"].Value;

                var currentExpression = BuildAnyExpression(parameter, property, operation, values);

                // Combine expressions using logical operators
                if (combinedExpression == null)
                {
                    combinedExpression = currentExpression;
                }
                else // Last condition, no logical operator expected
                {
                    combinedExpression = Expression.AndAlso(combinedExpression, currentExpression);
                }
            }

            if (combinedExpression == null) return query;

            // Build final lambda expression
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? search)
        {
            if (string.IsNullOrEmpty(search)) return query;

            var matches = FilterRegex.Matches(search);
            var parameter = Expression.Parameter(typeof(T), "x"); // Represents the entity in the query
            Expression? combinedExpression = null;

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var property = match.Groups["property"].Value;
                var operation = match.Groups["operation"].Value;
                var values = match.Groups["values"].Value;

                var currentExpression = BuildAnyExpression(parameter, property, operation, values);

                // Combine expressions using logical operators
                if (combinedExpression == null)
                {
                    combinedExpression = currentExpression;
                }
                else
                {
                    combinedExpression = Expression.OrElse(combinedExpression, currentExpression);
                }
            }

            if (combinedExpression == null) return query;

            // Build final lambda expression
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, string? include) where T : class
        {
            if (string.IsNullOrEmpty(include))
                return query;

            string[] includes = include.Split(',');
            foreach (string inc in includes)
            {
                query = query.Include(inc.Trim());
            }

            return query;
        }

        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, RequestQuery query) where T : class
        {
            return queryable
                .ApplyFilters(query.Filter)
                .ApplySearch(query.Search)
                .ApplySorting(query.Sort)
                .ApplyIncludes(query.Include);
        }

        private static Expression BuildExpression(Expression propertyExpression, string operation, string value)
        {
            var targetType = Nullable.GetUnderlyingType(propertyExpression.Type) ?? propertyExpression.Type;

            // null and not null handling
            if (operation.Equals("null", StringComparison.OrdinalIgnoreCase))
                return Expression.Equal(propertyExpression, Expression.Constant(null, propertyExpression.Type));

            if (operation.Equals("nnull", StringComparison.OrdinalIgnoreCase))
                return Expression.NotEqual(propertyExpression, Expression.Constant(null, propertyExpression.Type));

            // IN / NOT IN handling
            if (operation.Equals("in", StringComparison.OrdinalIgnoreCase) ||
                operation.Equals("notin", StringComparison.OrdinalIgnoreCase))
            {
                var values = value.Split(',')
                    .Select(v =>
                    {
                        if (targetType.IsEnum)
                        {
                            if (int.TryParse(v.Trim(), out int intValue))
                                return Enum.ToObject(targetType, intValue);
                            return Enum.Parse(targetType, v.Trim(), ignoreCase: true);
                        }

                        return Convert.ChangeType(v.Trim(), targetType);
                    })
                    .ToList();

                var listType = typeof(List<>).MakeGenericType(targetType);
                var list = Activator.CreateInstance(listType, new object[] { })!;
                var addMethod = listType.GetMethod("Add")!;

                foreach (var v in values)
                {
                    addMethod.Invoke(list, new[] { v });
                }

                var collection = Expression.Constant(list);
                var containsMethod = listType.GetMethod("Contains", new[] { targetType })!;

                var containsExpression = Expression.Call(collection, containsMethod, propertyExpression);

                return operation == "in"
                    ? containsExpression
                    : Expression.Not(containsExpression);
            }

            // Convert single value for other operations
            object? typedValue;
            try
            {
                typedValue = targetType == typeof(Guid)
                    ? Guid.Parse(value)
                    : targetType.IsEnum
                        ? Enum.Parse(targetType, value)
                        : Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert '{value}' to type {targetType.Name}", ex);
            }

            var constant = Expression.Constant(typedValue, propertyExpression.Type);

            return operation.ToLower() switch
            {
                "like" when propertyExpression.Type == typeof(string) =>
                    Expression.Call(
                        propertyExpression,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                        Expression.Constant(value)),

                "gte" => Expression.GreaterThanOrEqual(propertyExpression, constant),
                "lte" => Expression.LessThanOrEqual(propertyExpression, constant),
                "eq" => Expression.Equal(propertyExpression, constant),
                "neq" => Expression.NotEqual(propertyExpression, constant),

                _ => throw new InvalidOperationException($"Unsupported operation: {operation}")
            };
        }

        private static Expression BuildAnyExpression(Expression parameter, string propertyPath, string operation, string value)
        {
            var props = propertyPath.Split('.');
            Expression current = parameter;
            Type currentType = parameter.Type;

            for (int i = 0; i < props.Length; i++)
            {
                var propInfo = currentType.GetProperty(props[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null)
                    throw new InvalidOperationException($"Property '{props[i]}' not found on type {currentType.Name}");

                current = Expression.Property(current, propInfo);
                currentType = propInfo.PropertyType;

                // اگر کالکشن هست، باید Any بسازیم
                if (typeof(IEnumerable<>).IsAssignableFromGeneric(currentType) && currentType != typeof(string))
                {
                    var elementType = currentType.GetGenericArguments()[0];
                    var param = Expression.Parameter(elementType, "e");
                    var restPath = string.Join('.', props.Skip(i + 1));
                    var innerExpr = BuildExpression(ResolvePropertyPath(param, restPath), operation, value);
                    var lambda = Expression.Lambda(innerExpr, param);

                    return Expression.Call(typeof(Enumerable), "Any", new Type[] { elementType }, current, lambda);
                }
            }

            // اگر کالکشن نبود، فیلتر عادی بساز
            return BuildExpression(current, operation, value);
        }

        public static bool IsAssignableFromGeneric(this Type genericType, Type givenType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableFromGeneric(genericType, baseType);
        }
        
        private static Expression ResolvePropertyPath(Expression parameter, string propertyPath)
        {
            var properties = propertyPath.Split('.');
            Expression currentExpression = parameter;

            foreach (var property in properties)
            {
                currentExpression = Expression.PropertyOrField(currentExpression, property);
            }

            return currentExpression;
        }
    }
}