using Khidmah_Inventory.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Khidmah_Inventory.Infrastructure.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Applies filtering to the query based on filter criteria
    /// </summary>
    public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, List<FilterDto>? filters)
    {
        if (filters == null || !filters.Any())
            return query;

        foreach (var filter in filters)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? propertyAccess = parameter;
            Type propertyType = typeof(T);

            // Navigate through property chain (supports nested properties like "User.Email")
            foreach (var propName in filter.Column.Split('.'))
            {
                var propertyInfo = propertyType.GetProperty(
                    propName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    propertyAccess = null;
                    break;
                }

                propertyAccess = Expression.MakeMemberAccess(propertyAccess, propertyInfo);
                propertyType = propertyInfo.PropertyType;
            }

            if (propertyAccess == null) continue;

            Expression? expression = null;

            if (filter.Operator.ToLower() == "in")
            {
                expression = GetInExpression(propertyAccess, filter.Value, propertyType);
            }
            else
            {
                // Handle null values properly
                object? convertedValue = null;
                if (filter.Value != null)
                {
                    try
                    {
                        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                        convertedValue = ConvertToType(filter.Value, targetType);
                    }
                    catch
                    {
                        continue; // skip invalid filters
                    }
                }

                // Handle nullable vs non-nullable types
                Expression left = propertyAccess;
                Expression constant = Expression.Constant(convertedValue, propertyType);

                switch (filter.Operator.ToLower())
                {
                    case "=":
                    case "equals":
                        expression = Expression.Equal(left, constant);
                        break;

                    case "!=":
                    case "notequals":
                        expression = Expression.NotEqual(left, constant);
                        break;

                    case ">":
                        expression = Expression.GreaterThan(left, constant);
                        break;

                    case ">=":
                        expression = Expression.GreaterThanOrEqual(left, constant);
                        break;

                    case "<":
                        expression = Expression.LessThan(left, constant);
                        break;

                    case "<=":
                        expression = Expression.LessThanOrEqual(left, constant);
                        break;

                    case "equalsornull":
                        if (convertedValue == null)
                        {
                            // If value is null, just check for null
                            expression = Expression.Equal(left, Expression.Constant(null, left.Type));
                        }
                        else
                        {
                            // Check if property equals value OR property is null
                            var equalsExpr = Expression.Equal(left, constant);

                            // Only add null check if the property type is nullable
                            if (IsNullableType(propertyType))
                            {
                                var nullExpr = Expression.Equal(left, Expression.Constant(null, left.Type));
                                expression = Expression.OrElse(equalsExpr, nullExpr);
                            }
                            else
                            {
                                // For non-nullable types, just use equals
                                expression = equalsExpr;
                            }
                        }
                        break;

                    default:
                        expression = null;
                        break;
                }
            }

            if (expression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
                query = query.Where(lambda);
            }
        }

        return query;
    }

    /// <summary>
    /// Applies search to the query based on search configuration
    /// </summary>
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, SearchRequest? search)
    {
        if (search == null || string.IsNullOrWhiteSpace(search.Term) || !search.SearchFields.Any())
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? finalExpression = null;
        var searchTerm = search.Term;

        foreach (var field in search.SearchFields)
        {
            var property = typeof(T).GetProperty(
                field,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null) continue;

            // Only handle string properties for search
            if (property.PropertyType != typeof(string))
            {
                continue;
            }

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            Expression containsExpression = search.Mode switch
            {
                SearchMode.StartsWith =>
                    Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        null,
                        Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        propertyAccess,
                        Expression.Constant($"{searchTerm}%")),

                SearchMode.Contains =>
                    Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        null,
                        Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        propertyAccess,
                        Expression.Constant($"%{searchTerm}%")),

                SearchMode.EndsWith =>
                    Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        null,
                        Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        propertyAccess,
                        Expression.Constant($"%{searchTerm}")),

                SearchMode.ExactMatch =>
                    search.IsCaseSensitive
                        ? Expression.Equal(propertyAccess, Expression.Constant(searchTerm))
                        : Expression.Equal(
                            Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                            Expression.Constant(searchTerm.ToLower())),

                _ => throw new ArgumentOutOfRangeException(nameof(search.Mode))
            };

            finalExpression = finalExpression == null
                ? containsExpression
                : Expression.OrElse(finalExpression, containsExpression);
        }

        if (finalExpression == null) return query;

        var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Applies sorting to the query
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy)) return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = typeof(T).GetProperty(
            sortBy,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null) return query;

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var methodName = sortOrder?.ToLower() == "descending" ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// Applies pagination to the query
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationDto pagination)
    {
        return query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    /// <summary>
    /// Applies all filters, search, sorting, and pagination to the query
    /// </summary>
    public static IQueryable<T> ApplyFilterRequest<T>(this IQueryable<T> query, FilterRequest? filterRequest)
    {
        if (filterRequest == null)
            return query;

        // Apply filters
        if (filterRequest.Filters != null && filterRequest.Filters.Any())
        {
            query = query.ApplyFiltering(filterRequest.Filters);
        }

        // Apply search
        if (filterRequest.Search != null)
        {
            query = query.ApplySearch(filterRequest.Search);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrEmpty(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder);
        }

        // Apply pagination
        if (filterRequest.Pagination != null)
        {
            query = query.ApplyPagination(filterRequest.Pagination);
        }

        return query;
    }

    #region Helper Methods

    /// <summary>
    /// Checks if a type is nullable
    /// </summary>
    private static bool IsNullableType(Type type)
    {
        return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
    }

    /// <summary>
    /// Converts a value to the target type
    /// </summary>
    private static object? ConvertToType(object value, Type targetType)
    {
        if (value == null)
            return null;

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return null;

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, stringValue, ignoreCase: true);
        }

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return null;
            targetType = underlyingType;
        }

        // Handle boolean
        if (targetType == typeof(bool))
        {
            if (bool.TryParse(stringValue, out var boolValue))
                return boolValue;
            // Also handle "1", "0", "true", "false" strings
            if (stringValue == "1" || stringValue.ToLower() == "true")
                return true;
            if (stringValue == "0" || stringValue.ToLower() == "false")
                return false;
        }

        // Handle Guid
        if (targetType == typeof(Guid))
        {
            if (Guid.TryParse(stringValue, out var guidValue))
                return guidValue;
        }

        return Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Creates an "IN" expression for filtering
    /// </summary>
    private static Expression GetInExpression(Expression propertyAccess, object? values, Type propertyType)
    {
        IEnumerable<object?> valueList;

        // Handle different value formats (string, List, array, etc.)
        if (values is string stringValue)
        {
            // Split comma-separated strings like "1,2,3"
            valueList = stringValue
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => ConvertToType(v.Trim(), propertyType))
                .ToList();
        }
        else if (values is IEnumerable<object?> enumerable)
        {
            valueList = enumerable;
        }
        else
        {
            valueList = new List<object?> { values };
        }

        // Create a strongly typed List<T>
        var listType = typeof(List<>).MakeGenericType(propertyType);
        var list = Activator.CreateInstance(listType);
        var addMethod = listType.GetMethod("Add")!;

        foreach (var val in valueList)
        {
            // Convert each to propertyType, allowing nulls
            object? converted = val == null ? null : ConvertToType(val.ToString()!, propertyType);
            addMethod.Invoke(list, new[] { converted });
        }

        // Build: list.Contains(x.Property)
        var containsMethod = listType.GetMethod("Contains", new[] { propertyType })!;
        var listExpression = Expression.Constant(list);
        Expression containsExpression = Expression.Call(listExpression, containsMethod, propertyAccess);

        // If the list includes null, we also add: OR property == null
        bool includesNull = valueList.Any(v => v == null);
        if (includesNull)
        {
            var nullCheck = Expression.Equal(propertyAccess, Expression.Constant(null, propertyAccess.Type));
            containsExpression = Expression.OrElse(containsExpression, nullCheck);
        }

        return containsExpression;
    }

    #endregion
}

