using exam_system.Features.Diplomas.GetStudentDashboard.Orchestrators;
    using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
    using global::exam_system.Features.Diplomas.GetStudentDashboard.DTOs;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;


namespace exam_system.Features.Diplomas.GetStudentDashboard.Controllers
{

    [ApiController]
    [Route("api/students/me")]
    [Authorize(Roles = "Student")]
    public class StudentDashboardController(IMediator mediator)
        : ControllerBase
    {
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<ActionResult<StudentDashboardDto>> GetDashboard( CancellationToken ct)
        {
            var studentIdClaim = User.FindFirst("studentId")?.Value;

            if (studentIdClaim is null)
                return Unauthorized("StudentId claim is missing.");

            var studentId = Guid.Parse(studentIdClaim); // claim "studentId" mn el JWT

            var result = await mediator.Send(new GetStudentDashboardOrchestrator(studentId), ct);

            return Ok(result);
        }
    }
}
