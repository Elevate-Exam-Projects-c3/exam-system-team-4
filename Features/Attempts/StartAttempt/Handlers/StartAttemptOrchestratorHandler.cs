using exam_system.Common.Enums;
using exam_system.Features.Attempts.StartAttempt.Commands;
using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Attempts.StartAttempt.Queries;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Questions.Queries;
using exam_system.Features.Quizzes.Shared.Queries;
using exam_system.Features.Shared;
using MediatR;
using System.Security.Claims;

namespace exam_system.Features.Attempts.StartAttempt.Handlers
{
    public class StartAttemptOrchestratorHandler : IRequestHandler<StartAttemptOrchestrator, RequestResponse<StartAttemptResponseDto>>
    {
        private readonly IMediator _mediator;

        public StartAttemptOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<StartAttemptResponseDto>> Handle(StartAttemptOrchestrator request, CancellationToken cancellationToken)
        {
            //db=>get quiz =>status , Diploma Id, StartDate, EndtDate, DurationMinutes,MaxAttempts
            var quizRequest = await _mediator.Send(new GetQuizForStartAttemptQuery(request.QuizId), cancellationToken);

            //v=>validate quiz
            //v=>Check if the quiz exists and  
            
            if (quizRequest is null ||!quizRequest.Success || quizRequest.Data is null)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    quizRequest?.Message ?? "Failed to validate Quiz.",
                    quizRequest?.StatusCode ?? 500,
                    quizRequest?.Errors);
            }
            //is published. =>Status
            if (!(quizRequest.Data.Status == QuizStatus.Published))
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "Quiz is not published.",
                    409,
                    new Dictionary<string, string[]>
                    {
                        ["Quiz"] = ["The quiz must be published before starting an attempt."]
                    });
            }

            //v=>Check dates
            //Is Date Ended ... Is Start Date Is not Started yet
            var now= DateTime.UtcNow;
            if (quizRequest.Data!.EndDate <= now)
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "Quiz has already ended.",
                    409,
                    new Dictionary<string, string[]>
                    {
                        ["EndDate"] = ["Quiz has already ended."]
                    });

            if (quizRequest.Data.StartDate > now)
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "Quiz has not started yet.",
                    409,
                    new Dictionary<string, string[]>
                    {
                        ["StartDate"] = ["Quiz has not started yet."]
                    });
         
            //db=> Is student enroll at quiz Diploma
            var enrollmentResult = await _mediator.Send(new IsStudentEnrolledInDiplomaQuery(request.studentId, quizRequest.Data.DiplomaId));
            if (enrollmentResult is null|| !enrollmentResult.Success)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    enrollmentResult?.Message ?? "Failed to validate student enrollment.",
                    enrollmentResult?.StatusCode??500,
                    enrollmentResult?.Errors);
            }

            if (!enrollmentResult.Data)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "Student is not enrolled in this diploma.",
                    403,
                    new Dictionary<string, string[]>
                    {
                        ["Quiz"] = ["Student is not enrolled in quiz diploma."]
                    });
            }
            ////Send request
            //db=>Check for an existing InProgress attempt for the current student and quiz.
            var inProgressAttempt=await _mediator.Send(new GetInProgressQuizAttemptQuery(request.studentId, quizRequest.Data.QuizId));
            //v=>existing attempt is found → return it and don't create another.
            if (inProgressAttempt.Success)
                return inProgressAttempt;
            /////////////////////////
            

            //db=>Count Submitted +TimedOut attempts for this user and quiz.
            var submittedAndTimeoutAttemptsCountResult = await _mediator.Send(new GetSubmittedAndTimedOutAttemptsCountQuery(request.studentId, request.QuizId));
            //v=>Check MaxAttempts
            if (submittedAndTimeoutAttemptsCountResult is null||!submittedAndTimeoutAttemptsCountResult.Success)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    submittedAndTimeoutAttemptsCountResult?.Message ?? "Failed to count attempts.",
                    submittedAndTimeoutAttemptsCountResult?.StatusCode??500,
                    submittedAndTimeoutAttemptsCountResult?.Errors);
            }
            var submittedAndTimeoutAttemptsCount = submittedAndTimeoutAttemptsCountResult.Data;
            if (quizRequest.Data.MaxAttempts is not null && 
                quizRequest.Data.MaxAttempts<= submittedAndTimeoutAttemptsCount)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "Attempts exhausted!",
                    403,
                    new Dictionary<string, string[]>
                    {
                        ["Attempts"] = ["Attempts exhausted."]
                    });
            }
            //db=>CreateQuizAttemptCommand //create quiz attempt
           var isCreatedResponse=  await _mediator.Send(new CreateQuizAttemptCommand(
                                           request.studentId,
                                           request.QuizId,
                                           StartTime: now,
                                           Deadline: now.AddMinutes(quizRequest.Data.DurationMinutes),
                                           attemptStatus: AttemptStatus.InProgress
                                           ));
            if (isCreatedResponse is null || !isCreatedResponse.Success)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    isCreatedResponse?.Message ?? "Failed to create quiz attempt.",
                    isCreatedResponse?.StatusCode ?? 500,
                    isCreatedResponse?.Errors);
            }
            
            //db=>Result=>Load Quiz Questions + Options
           
            var attempt = await _mediator.Send(new GetInProgressQuizAttemptQuery(request.studentId, quizRequest.Data.QuizId));
            if(attempt is null  || !attempt.Success || attempt.Data is null)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail(
                        attempt?.Message ?? "In-progress attempt not found.",
                        attempt?.StatusCode ?? 404,
                        attempt?.Errors);
            }
            var questionsAndOptionsResult = await _mediator.Send(new GetQuizQuestionsAndOptionsForStartAttemptQuery(quizRequest.Data.QuizId),
                                            cancellationToken);
            if (questionsAndOptionsResult is null || questionsAndOptionsResult.Data is null)
            {
                return RequestResponse<StartAttemptResponseDto>.Fail("Failed to retrieve quiz questions and options.");
            }
                FisherYatesShuffle<AttemptQuestionDto>(questionsAndOptionsResult.Data);
            foreach (var question in questionsAndOptionsResult.Data)
            {
                FisherYatesShuffle(question.Options);
            }
            attempt.Data.Questions=questionsAndOptionsResult.Data;
            return RequestResponse< StartAttemptResponseDto>.Ok(attempt.Data);

        }

        private void FisherYatesShuffle<T>(IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(0, i + 1);

                (items[i], items[j]) = (items[j], items[i]);
            }
        }
    }
}
