using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.Register.Queries
{
    public record EmailExistsQuery(string Email) :IRequest<RequestResponse<bool>>
    {

    }
}
