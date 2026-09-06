using exam_system.Domain.Common;
using System.Linq.Expressions;

namespace exam_system.Persistence.DataAccess;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes);
    IQueryable<T> GetAll();
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    void HardDelete(T entity);
    Task<int> CountAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>? criteria = null);
}
