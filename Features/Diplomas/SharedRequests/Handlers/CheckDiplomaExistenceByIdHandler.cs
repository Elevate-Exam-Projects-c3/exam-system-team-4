using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.SharedRequests.Handlers
{
    public class CheckDiplomaExistenceByIdHandler : IRequestHandler<CheckDiplomaExistenceById, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepo;

        public CheckDiplomaExistenceByIdHandler(IGenericRepository<Diploma> DiplomaRepo)
        {
            _diplomaRepo = DiplomaRepo;
        }

        public async Task<RequestResponse<bool>> Handle(CheckDiplomaExistenceById request, CancellationToken cancellationToken)
        {
           var isExist= await _diplomaRepo.GetAll().AnyAsync(diploma => diploma.Id == request.diplomaId, cancellationToken);
           return RequestResponse<bool>.Ok(isExist);
        }
    }
}
