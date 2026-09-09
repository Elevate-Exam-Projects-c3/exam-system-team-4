namespace exam_system.Features.Shared.Cqrs
{
    public interface ITransactionResult
    {
        bool ShouldCommit { get; }
    }
}
