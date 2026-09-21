using exam_system.Common.Enums;
using exam_system.Features.Attempts.SubmitAttempt.Commands;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Attempts.SubmitAttempt.Orchestrators;
using exam_system.Features.Attempts.SubmitAttempt.Queries;
using exam_system.Features.Attempts.UpdateAttemptStatus.Commands;
using exam_system.Features.Shared;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace exam_system.Features.Attempts.SubmitAttempt.Handlers
{
    public class SubmitQuizAttemptOrchestratorHandler :
        IRequestHandler<SubmitQuizAttemptOrchestrator, RequestResponse<SubmitAttemptResponseDto>>
    {
        private readonly IMediator _mediator;

        public SubmitQuizAttemptOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<SubmitAttemptResponseDto>> Handle(SubmitQuizAttemptOrchestrator request, CancellationToken cancellationToken)
        {
            //Get attempt status , deadline, quizId, StudentId, questionId, 
            var attemptResponse=await _mediator.Send(new GetQuizAttemptForSubmissionQuery(request.attemptId), cancellationToken);
            if (!attemptResponse.Success || attemptResponse.Data is null)
            {
                return RequestResponse<SubmitAttemptResponseDto>.Fail(
                    attemptResponse.Message,
                    attemptResponse.StatusCode,
                    attemptResponse.Errors);
            }


            var attempt = attemptResponse.Data;

            // 2. Attempt must still be in progress
            if (attempt.Status != AttemptStatus.InProgress)
            {
                return RequestResponse<SubmitAttemptResponseDto>.Fail(
                    "Attempt cannot be submitted because it is no longer in progress.",
                    StatusCodes.Status409Conflict);
            }

            // 3. Check deadline
            var now = DateTime.UtcNow;

            if (now >= attempt.Deadline)
            {
               await _mediator.Send(new UpdateAttemptStatusCommand(attempt.AttemptId,AttemptStatus.TimedOut), cancellationToken);
                return RequestResponse<SubmitAttemptResponseDto>.Fail("The attempt deadline has expired.", 409);
            }
            //get count of correct answers and Total answers
            var AttemptScoreResult= await _mediator.Send(new GetAttemptScoreQuery(QuizId: attempt.QuizId,
                                                                    AttemptId:attempt.AttemptId),
                                                        cancellationToken);
            if(AttemptScoreResult is null || !AttemptScoreResult.Success|| AttemptScoreResult.Data is null)
            {
                return RequestResponse<SubmitAttemptResponseDto>.Fail("Failed to calculate attempt score.");
            }

            // 
            SubmitAttemptResponseDto submitAttemptResponseDto = new SubmitAttemptResponseDto
            {
                AttemptId = attempt.AttemptId,
                QuizId = attempt.QuizId,
                TotalQuestions = AttemptScoreResult.Data.TotalQuestions,
                CorrectAnswers = AttemptScoreResult.Data.CorrectAnswers,
                SubmittedAt = DateTime.UtcNow,
                AttemptStatus= AttemptStatus.Submitted,

            };
            //Scoring formula: (number of correct answers ÷ total questions) *100
            submitAttemptResponseDto.Score= ((decimal)submitAttemptResponseDto.CorrectAnswers /(decimal) submitAttemptResponseDto.TotalQuestions) * 100m;
            submitAttemptResponseDto.Passed = (attempt.PassScore <= submitAttemptResponseDto.Score);

            //submit
            var submitResult = await _mediator.Send(new SubmitQuizAttemptCommand(AttemptId: submitAttemptResponseDto.AttemptId,
                                                                                AttemptStatus: submitAttemptResponseDto.AttemptStatus,
                                                                                Score: submitAttemptResponseDto.Score,
                                                                                Passed: submitAttemptResponseDto.Passed,
                                                                                SubmittedAt: submitAttemptResponseDto.SubmittedAt),
                                                                                cancellationToken);
            if (!submitResult.Success )
            {
                return RequestResponse<SubmitAttemptResponseDto>.Fail(
                    submitResult.Message,
                    submitResult.StatusCode,
                    submitResult.Errors);
            }


            return RequestResponse<SubmitAttemptResponseDto>.Ok(submitAttemptResponseDto);
        }
    }
}
