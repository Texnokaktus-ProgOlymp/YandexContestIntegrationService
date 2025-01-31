using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class YandexAuthenticationServiceImpl(ITokenService tokenService, IYandexAuthenticationService yandexAuthenticationService) : YandexAuthenticationService.YandexAuthenticationServiceBase
{
    public override async Task<IsServiceAuthenticatedResponse> IsServiceAuthenticated(Empty request, ServerCallContext context)
    {
        var token = await tokenService.GetAccessTokenAsync();

        return new()
        {
            Result = token is not null
        };
    }

    public override async Task<Empty> AuthenticateService(AuthenticateServiceRequest request, ServerCallContext context)
    {
        var tokenResponse = await yandexAuthenticationService.GetAccessTokenAsync(request.Code);
        await tokenService.RegisterTokenAsync(tokenResponse);

        return new();
    }
}
