using System.Linq.Expressions;
using System.Reflection;

namespace TaskTrackerClean.Infrastructure.Helpers
{
    public static class SortingHelper
    {
        public static Expression<Func<T, object>> GetSortBy<T>(string? sortByText)
        {
            // Use "Id" as default if input is null/empty
            var propertyName = string.IsNullOrWhiteSpace(sortByText) ? "Id" : sortByText;

            var param = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // If property not found, fallback to "Id"
            if (property == null)
            {
                property = typeof(T).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    throw new ArgumentException($"Neither '{sortByText}' nor fallback 'Id' property found on type '{typeof(T).Name}'.");
            }

            var propertyAccess = Expression.Property(param, property);
            var converted = Expression.Convert(propertyAccess, typeof(object));

            return Expression.Lambda<Func<T, object>>(converted, param);
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>> GetSortAs<T>(string? sortAsText, Expression<Func<T, object>> sortBy)
        {
            var direction = string.IsNullOrWhiteSpace(sortAsText) ? "desc" : sortAsText.ToLower();

            return direction switch
            {
                "asc" => query => query.OrderBy(sortBy),
                "desc" => query => query.OrderByDescending(sortBy),
                _ => query => query.OrderByDescending(sortBy)
            };
        }
    }
}