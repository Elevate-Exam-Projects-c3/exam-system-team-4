using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.GetDiplomaDetail.DTOs;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetMyDiplomasQuery(Guid StudentId) : IRequest<RequestResponse<List<GetDiplomaDto>>>;

public class GetMyDiplomasQueryHandler : IRequestHandler<GetMyDiplomasQuery, RequestResponse<List<GetDiplomaDto>>>
{
    private readonly IGenericRepository<StudentEnrollment> _enrollments;

    public GetMyDiplomasQueryHandler(IGenericRepository<StudentEnrollment> enrollments)
    {
        _enrollments = enrollments;
    }

    public async Task<RequestResponse<List<GetDiplomaDto>>> Handle(GetMyDiplomasQuery request, CancellationToken ct)
    {
        var diplomas = await _enrollments.GetAll()
            .AsNoTracking()
            .Where(e => e.StudentId == request.StudentId && !e.Diploma.IsDeleted)
            .Select(e => new GetDiplomaDto
            {
                DiplomaId = e.DiplomaId,
                Title = e.Diploma.Title,
                Description = e.Diploma.Description
            })
            .ToListAsync(ct);

        return RequestResponse<List<GetDiplomaDto>>.Ok(diplomas, "Retrieved", 200);
    }
}