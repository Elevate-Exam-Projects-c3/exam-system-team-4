using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using exam_system.Domain.Common;
using exam_system.Persistence.Context;

namespace exam_system.Persistence.DataAccess;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id,  cancellationToken);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public void  Add(T entity)
    {
         _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
         _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

 
    // Soft Delete - marks as deleted but keeps in database
    public void Delete(T entity)
    {
        _dbSet.Attach(entity);
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        // Mark the entity as modified so EF will update it
        _context.Entry(entity).State = EntityState.Modified;
    }

    // Hard Delete - physically removes from database
    public void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.RemoveRange(entities);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken,Expression<Func<T, bool>>? criteria = null)
    {
        if (criteria == null)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }

        return await _dbSet.CountAsync(criteria,cancellationToken);
    }

    

    

}
