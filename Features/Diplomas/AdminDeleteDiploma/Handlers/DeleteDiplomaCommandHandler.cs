using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Handlers
{
    public class DeleteDiplomaCommandHandler : IRequestHandler<DeleteDiplomaCommand, bool>
    {
        private readonly IGenericRepository<Diploma> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDiplomaCommandHandler(IGenericRepository<Diploma> repository , IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = await _repository.GetByIdAsync(request.Id , cancellationToken);

            if (diploma == null) 
            { 
                throw new Exception("Diploma Not Found");
            }
            if (diploma.Enrollments.Any()) 
            { 
                throw new Exception("Cannot delete diploma with active enrollments");
            }
            
              
            diploma.IsDeleted = true;
            diploma.UpdatedAt = DateTime.Now;

           await _unitOfWork.SaveChangesAsync(cancellationToken);
           return true;

        }

       
    }
}
