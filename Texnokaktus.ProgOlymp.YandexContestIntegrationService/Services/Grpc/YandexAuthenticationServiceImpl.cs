using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexOAuthClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class YandexAuthenticationServiceImpl(IAuthService authService) : YandexAuthenticationService.YandexAuthenticationServiceBase
{
    public override async Task<IsServiceAuthenticatedResponse> IsServiceAuthenticated(Empty request, ServerCallContext context)
    {
        var token = await authService.GetAccessTokenAsync("DEFAULT");

        return new()
        {
            Result = token is not null
        };
    }

    public override Task<GetOAuthUrlResponse> GetOAuthUrl(GetOAuthUrlRequest request, ServerCallContext context) =>
        Task.FromResult<GetOAuthUrlResponse>(new()
        {
            Result = authService.GetOAuthUrl(request.RedirectUrl)
        });

    public override async Task<Empty> AuthenticateService(AuthenticateServiceRequest request, ServerCallContext context)
    {
        await authService.AuthorizeAsync("DEFAULT", request.Code);

        return new();
    }
}
