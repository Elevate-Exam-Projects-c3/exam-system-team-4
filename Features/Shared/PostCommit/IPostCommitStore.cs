using MediatR;

namespace exam_system.Features.Shared.PostCommit
{
    public interface IPostCommitStore

    {
        void Add(INotification notification);

        IReadOnlyList<INotification> TakeAll();

        void Clear();
    }
}
