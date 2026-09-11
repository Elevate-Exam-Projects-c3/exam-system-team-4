using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.BrowseDiplomas.DTOs;
using exam_system.Features.Diplomas.BrowseDiplomas.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Handlers
{



    public class BrowseDiplomasQueryHandler : IRequestHandler<BrowseDiplomasQuery, RequestResponse<PaginatedResult<BrowseDiplomaDto>>>
    {
    private readonly IGenericRepository<Diploma> _repository;
    private readonly IUnitOfWork _unitOfWork;

        public BrowseDiplomasQueryHandler(IGenericRepository<Diploma> repository, Persistence.DataAccess.IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<PaginatedResult<BrowseDiplomaDto>>> Handle(BrowseDiplomasQuery request, CancellationToken cancellationToken)
        {
            //get diplomas with published quizzes Only
            var diplomasWithQuizzes =  _repository.GetAll()
                .Where(d => d.Quizzes.Any(q => q.Status == QuizStatus.Published));

            var TotalCount = diplomasWithQuizzes.CountAsync(cancellationToken);

            var items = diplomasWithQuizzes
                .OrderByDescending(d => d.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize).Select(i => new BrowseDiplomaDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    TotalQuizzes = i.Quizzes.Count(i => i.Status == QuizStatus.Published),
                    CompletedQuizzes = request.StudentID.HasValue
                        ? i.Quizzes.Count(q =>
                            q.Attempts.Any(a =>
                            a.StudentId == request.StudentID.Value &&
                            (a.Status == AttemptStatus.Submitted ||
                             a.Status == AttemptStatus.TimedOut)))
                                : 0

                }).ToListAsync(cancellationToken);

            var results = new PaginatedResult<BrowseDiplomaDto>(items, TotalCount, request.PageNumber, request.PageSize);

            return RequestResponse<PaginatedResult<BrowseDiplomaDto>>.Ok(results);

        }
    }
}
