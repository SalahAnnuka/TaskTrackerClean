using System.Linq.Expressions;

namespace TaskTrackerClean.Domain.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(int id);
    Task<TEntity> FindByIdAsync(
        int id,
        params Expression<Func<TEntity, object>>?[] includes);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<(IEnumerable<TEntity> Items, int TotalPages, int TotalItems)> FindWithIncludesAsync(
        int page,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes);
    Task<(IEnumerable<TEntity> Items, int TotalPages, int TotalItems)> FindWithIncludesAsync(
    int page,
    int pageSize,
    string? sortBy,
    string? sortAs,
    Expression<Func<TEntity, bool>>? predicate = null,
    params Expression<Func<TEntity, object>>?[] includes);

    Task<TEntity> RecoverAsync(int id);
}






