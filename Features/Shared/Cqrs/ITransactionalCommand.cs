namespace exam_system.Features.Shared.Cqrs
{
    public interface ITransactionalCommand<TResponse>
     : ICommand<TResponse>
    {
    }
}
