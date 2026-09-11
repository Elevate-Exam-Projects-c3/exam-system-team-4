using exam_system.Features.Diplomas.AdminCreateDiploma.ViewModels;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Diplomas.AdminUpdateDiploma.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateDiplomaController : ControllerBase
    {
        IMediator _mediator;
        public UpdateDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update( Guid id,[FromBody] UpdateDiplomaViewModel viewModel)
        {
            var command = new UpdateDiplomaCommand(
                id,
                viewModel.Title,
                viewModel.Description);
            var result = await _mediator.Send(command);

            return Ok("Diploma updated successfully");
        }

    }
}
