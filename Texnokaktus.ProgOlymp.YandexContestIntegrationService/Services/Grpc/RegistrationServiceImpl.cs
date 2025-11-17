using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class RegistrationServiceImpl(IRegistrationService registrationService, ILogger<RegistrationServiceImpl> logger) : RegistrationService.RegistrationServiceBase
{
    public override async Task<Empty> RegisterParticipant(RegisterParticipantRequest request, ServerCallContext context)
    {
        try
        {
            await registrationService.RegisterUserAsync(request.ContestStageId, request.YandexIdLogin, request.DisplayName, request.ParticipantId);
            return new();
        }
        catch (InvalidYandexUserException e)
        {
            throw new RpcException(new(StatusCode.InvalidArgument, e.Message, e));
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while registering the user to the contest");
            throw;
        }
    }

    public override async Task<Empty> UnregisterParticipant(UnregisterParticipantRequest request, ServerCallContext context)
    {
        try
        {
            await registrationService.UnregisterUserAsync(request.ContestStageId, request.ParticipantId);
            return new();
        }
        catch (UserIsNotRegisteredException e)
        {
            throw new RpcException(new(StatusCode.NotFound, e.Message, e));
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while registering the user to the contest");
            throw;
        }
    }
}
