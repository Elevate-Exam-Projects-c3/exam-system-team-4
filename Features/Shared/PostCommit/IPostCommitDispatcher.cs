namespace exam_system.Features.Shared.PostCommit
{
    public interface IPostCommitDispatcher
    {
        Task DispatchAsync(CancellationToken cancellationToken = default);
    }
}
