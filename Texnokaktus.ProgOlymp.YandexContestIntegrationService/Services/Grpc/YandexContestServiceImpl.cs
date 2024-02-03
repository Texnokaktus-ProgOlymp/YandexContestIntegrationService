using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class YandexContestServiceImpl(IRegistrationService registrationService, ILogger<YandexContestServiceImpl> logger) : YandexContestService.YandexContestServiceBase
{
    public override async Task<RegisterParticipantResponse> RegisterParticipant(RegisterParticipantRequest request, ServerCallContext context)
    {
        try
        {
            await registrationService.RegisterUserAsync(request.ContestStageId, request.YandexIdLogin);
            return new();
        }
        catch (RpcApplicationException e)
        {
            return new()
            {
                Error = new()
                {
                    Type = e.ErrorType,
                    Message = e.Message
                }
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while registering the user to the contest");
            return new()
            {
                Error = new()
                {
                    Type = ErrorType.Generic,
                    Message = e.Message
                }
            };
        }
    }

    public override async Task<UnregisterParticipantResponse> UnregisterParticipant(UnregisterParticipantRequest request, ServerCallContext context)
    {
        try
        {
            await registrationService.UnregisterUserAsync(request.ContestStageId, request.YandexIdLogin);
            return new();
        }
        catch (RpcApplicationException e)
        {
            return new()
            {
                Error = new()
                {
                    Type = e.ErrorType,
                    Message = e.Message
                }
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while registering the user to the contest");
            return new()
            {
                Error = new()
                {
                    Type = ErrorType.Generic,
                    Message = e.Message
                }
            };
        }
    }
}
