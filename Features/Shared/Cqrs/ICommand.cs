using MediatR;

namespace exam_system.Features.Shared.Cqrs
{
    public interface ICommand<TResponse>
     : IRequest<TResponse>
    {
    }
}
