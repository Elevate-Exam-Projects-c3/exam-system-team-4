using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Diplomas.AdminCreateDiploma.DTO;
using exam_system.Features.Diplomas.AdminCreateDiploma.Validators;
using exam_system.Features.Diplomas.AdminCreateDiploma.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateDiplomaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CreateDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }

      

        [HttpPost]
        public async Task<IActionResult> CreateDiploma([FromBody] UpdateDiplomaViewModel viewModel)
        {
           
            //send method here excute the request then the handler will be called to handle the request and return the result
            var createdDiploma = await _mediator.Send(new CreateDiplomaCommand(viewModel.Title, viewModel.Description));
           
            //return the created diploma
            return Ok("Diploma Created Successfully");
        }
    }
}
