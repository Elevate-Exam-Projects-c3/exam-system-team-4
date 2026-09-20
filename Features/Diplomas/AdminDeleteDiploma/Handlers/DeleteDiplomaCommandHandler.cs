using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Handlers
{
    public class DeleteDiplomaCommandHandler : IRequestHandler<DeleteDiplomaCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<Diploma> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDiplomaCommandHandler(IGenericRepository<Diploma> repository , IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Unit>> Handle(DeleteDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = await _repository.GetByIdAsync( request.Id, cancellationToken, d => d.Enrollments);

            if (diploma == null || diploma.IsDeleted) 
            { 
                return RequestResponse<Unit>.Fail("Diploma Is Not Found" , 400);
            }

            if (diploma.Enrollments.Any()) 
            { 
                return RequestResponse<Unit>.Fail("Cannot delete diploma with active enrollments" , 409);
            }

            _repository.Delete(diploma);

           await _unitOfWork.SaveChangesAsync(cancellationToken);
           return RequestResponse<Unit>.Ok(Unit.Value , "Diploma Deleted Successfully");

        }

       
    }
}
