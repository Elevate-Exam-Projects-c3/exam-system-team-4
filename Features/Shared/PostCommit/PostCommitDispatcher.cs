using MediatR;

namespace exam_system.Features.Shared.PostCommit
{
    public class PostCommitDispatcher : IPostCommitDispatcher
    {
        private readonly IPostCommitStore _postCommitStore;
        private readonly IPublisher _publisher;

        public PostCommitDispatcher(
            IPostCommitStore postCommitStore,
            IPublisher publisher)
        {
            _postCommitStore = postCommitStore;
            _publisher = publisher;
        }

        public async Task DispatchAsync(CancellationToken cancellationToken = default)
        {
            var notifications = _postCommitStore.TakeAll();

            foreach (var item in notifications)
            {
                await _publisher.Publish(item,cancellationToken);
            }
        }
    
    }
}
