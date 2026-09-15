using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Diplomas.AdminUpdateDiploma.DTO;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Handlers
{
    public class UpdateDiplomaHandler : IRequestHandler<UpdateDiplomaCommand, Unit>
    {
        #region Dependency Injection
        //get diploma form database using GenericRepository 
        private readonly IGenericRepository<Diploma> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiplomaHandler(
            IGenericRepository<Diploma> repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        #endregion


        public async Task<Unit> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)

        {
            //get diploma by id 
            var diploma = await _repository.GetByIdAsync(request.Id, cancellationToken);

            //check if diploma is null or deleted
            if (diploma == null || diploma.IsDeleted)
            {
                throw new KeyNotFoundException("Diploma not found");
            }

            //if not null update the diploma title and description
            //replace the title and description with the new values from the request
            diploma.Title = request.Title;
            diploma.Description = request.Description;
            diploma.UpdatedAt =  DateTime.UtcNow;

            //then save changes to the database
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            //then return the updated diploma as UpdateDiplomaDto
            return Unit.Value;

        }
    }
}


