using MediatR;

namespace exam_system.Features.Shared.PostCommit
{
    public class PostCommitStore : IPostCommitStore
    {
        private readonly List<INotification> _notifications = new();

        public void Add(INotification notification)
        {
            _notifications.Add(notification);
        }

        public IReadOnlyList<INotification> TakeAll()
        {
            var notifications = _notifications.ToArray();

            _notifications.Clear();

            return notifications;
        }

        public void Clear()
        {
            _notifications.Clear();
        }
    }
}
