using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerClean.Domain.Entities;

namespace TaskTrackerClean.Infrastructure.Helpers
{
    public static class PaginationHelper<TEntity> where TEntity : BaseEntity<TEntity>
    {
        public static IQueryable<TEntity> PrepareQuery(
            IQueryable<TEntity> dbSet,
            Expression<Func<TEntity, bool>>? predicate,
            Expression<Func<TEntity, object>>?[] includes,
            string? sortBy,
            string? sortAs)
        {
            var query = dbSet.Where(e => !e.IsDeleted);

            if (predicate != null)
                query = query.Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (include != null)
                        query = query.Include(include);
                }
            }

            var sortExpression = SortingHelper.GetSortBy<TEntity>(sortBy);
            var sortFunc = SortingHelper.GetSortAs(sortAs, sortExpression);

            return sortFunc(query);
        }

        public static (int page, int pageSize, int totalPages) NormalizePagination(int page, int pageSize, int totalPages, int totalItems)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            totalPages = Math.Min(page, totalPages);
            return (page, pageSize, totalPages);
        }


    }
}
