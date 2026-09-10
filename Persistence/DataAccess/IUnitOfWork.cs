namespace exam_system.Persistence.DataAccess;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(
        CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(
        CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default);

    Task EndTransactionAsync();

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
