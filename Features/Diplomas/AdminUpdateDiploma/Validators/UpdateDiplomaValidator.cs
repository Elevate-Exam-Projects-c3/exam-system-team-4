using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using FluentValidation;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Validators
{
    public class UpdateDiplomaValidator
     : AbstractValidator<UpdateDiplomaCommand>
    {
        public UpdateDiplomaValidator()
        {
            //add same restrictions as in CreateDiplomaValidator
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
