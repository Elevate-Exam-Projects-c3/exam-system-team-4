using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using FluentValidation;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Validators
{
    //this validator class will validate the CreateDiplomaCommand properties
    //use validator to seperate the validation logic from the command handler (business logic)
    public class CreateDiplomaValidator : AbstractValidator<CreateDiplomaCommand>
    {
        public CreateDiplomaValidator()
        {
            //Add rule to title property
            RuleFor(x => x.Title)
                 .NotEmpty()
                 .MinimumLength(3)
                 .MaximumLength(200);
            RuleFor(x => x.Description)
                .MaximumLength(1000);
                
        }
    }
}
