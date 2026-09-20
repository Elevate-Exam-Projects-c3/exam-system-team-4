using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Diplomas.AdminCreateDiploma.DTO;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Handlers
{
    //create handler for creating a diploma command that returns the id (unique) of the created diploma
    public class CreateDiplomaCommandHandler : IRequestHandler<CreateDiplomaCommand, RequestResponse<Unit>>
    {

        #region Dependency Injection
        private readonly IGenericRepository<Diploma> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiplomaCommandHandler(
            IGenericRepository<Diploma> repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        #endregion



        //handle method is the method that will be called when the command is sent to the mediator _mediator.send() , it will call the orchestrator to create the diploma and return the id of the created diploma
        public async Task<RequestResponse<Unit>> Handle(CreateDiplomaCommand request, CancellationToken cancellationToken)
        {
            //check early (from reviews)

            if (request.Title == null || request.Title.Length < 3)
            {
                return RequestResponse<Unit>.Fail("Title Must Be Between 3 To 200 Characters");
            }
            if ( request.Description?.Length > 1000)
            {
                return RequestResponse<Unit>.Fail("Description Must Be Less That 1000 Characters");
            }

            else
            {
                //step 1 : create Diploma
                //When the command is executed, create a new diploma in diploma entity
                var diploma = new Diploma
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                //step 2 : add it to database
                //add the created diploma to database using repository
                _repository.Add(diploma);
                //step 3 : save changes to database
                //add to database using unit of work
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                //step 4 : return the created diploma
               return RequestResponse<Unit>.Created(Unit.Value , "Diploma Created Successfully");
            }

           
        }
    }
} 
