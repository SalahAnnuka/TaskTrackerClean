using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskTrackerClean.Domain.Entities;
using TaskTrackerClean.Domain.Interfaces;
using TaskTrackerClean.Infrastructure.Helpers;

namespace TaskTrackerClean.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : BaseEntity<TEntity>
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return null!;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return entity!;
    }

    public async Task<TEntity> FindByIdAsync(
        int id,
        params Expression<Func<TEntity, object>>?[] includes)
    {
        var query = _dbSet.Where(e => !e.IsDeleted && e.Id == id);
        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (include != null)
                    query = query.Include(include);
            }
        }

        var result = await query.FirstOrDefaultAsync();

        return result!;
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = _dbSet.Where(e => !e.IsDeleted).AsNoTracking();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<TEntity> Items, int TotalPages, int TotalItems)> FindWithIncludesAsync(
    int page,
    int pageSize,
    string? sortBy,
    string? sortAs,
    Expression<Func<TEntity, bool>>? predicate = null,
    params Expression<Func<TEntity, object>>?[] includes)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (include != null)
                    query = query.Include(include);
            }
        }

        // Apply sorting using helper
        var sortExpression = SortingHelper.GetSortBy<TEntity>(sortBy);
        var sortFunc = SortingHelper.GetSortAs(sortAs, sortExpression);
        query = sortFunc(query);

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            throw new KeyNotFoundException($"Page number ({page}) exceeds total pages ({totalPages}) for page size ({pageSize}).");
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalPages, totalItems);
    }


    public async Task<TEntity> RecoverAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return null!;

        entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return entity!;
    }

    public Task<(IEnumerable<TEntity> Items, int TotalPages, int TotalItems)> FindWithIncludesAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includes)
    {
        throw new NotImplementedException();
    }
}
