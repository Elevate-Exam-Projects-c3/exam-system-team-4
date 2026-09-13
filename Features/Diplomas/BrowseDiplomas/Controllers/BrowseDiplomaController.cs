using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.BrowseDiplomas.Queries;
using exam_system.Features.Diplomas.BrowseDiplomas.ViewModels;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowseDiplomaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrowseDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DiplomaItemsListViewModel>>> GetDiploma ([FromQuery]  int Pagesize = 5 , [FromQuery] int PageNumber = 1 , [FromQuery] Guid? studentID = null , CancellationToken ct = default)
        {
            var result = await _mediator.Send(new BrowseDiplomasQuery(studentID, Pagesize, PageNumber), ct);
            var viewModel = result.Data.Items.Select(items => new DiplomaItemsListViewModel
            {
               DiplomaID = items.Id,
               Title = items.Title,
               Description = items.Description,
               CompletedQuizzes = items.CompletedQuizzes,
               TotalQuizzes = items.TotalQuizzes,
            }).ToList();

            var PaginatedResult = new PaginatedResult<DiplomaItemsListViewModel>(viewModel, result.Data.TotalCount, result.Data.PageSize, result.Data.PageIndex);

            return Ok(EndpointResponse<PaginatedResult<DiplomaItemsListViewModel>>.Ok(PaginatedResult));
        }
    }
}
