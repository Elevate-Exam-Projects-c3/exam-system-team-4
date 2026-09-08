using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Diplomas.AdminCreateDiploma.DTO;
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

        //[HttpPost]
        //public async Task<IActionResult> CreateDiploma([FromBody] CreateDiplomaCommand command)
        //{
        //    // excute the command (create diploma) using MediatR | object of type CreateDiplomaCommand will be passed to the handler
        //    //mediator excutes requests but it searches for the handler of this request
        //    var diploma = await _mediator.Send(new CreateDiplomaCommand() 
        //    { 
        //        Title = command.Title,
        //    Description = command.Description,
        //    });
        //    //return the created diploma
        //    return Ok(diploma);
        //}

        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] CreateDiplomaDto dto)
        {
            //send method here excute the request then the handler will be called to handle the request and return the result
            var createdDiploma = await _mediator.Send(new CreateDiplomaCommand()
            {
                //map the dto to the command
                Title = dto.Title,
                Description = dto.Description
            });
            //return the created diploma
            return Ok(createdDiploma);
        }
    }
}
