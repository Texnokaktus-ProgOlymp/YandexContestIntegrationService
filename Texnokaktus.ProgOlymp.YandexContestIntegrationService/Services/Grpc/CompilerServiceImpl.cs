using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class CompilerServiceImpl(ContestClient client) : CompilerService.CompilerServiceBase
{
    public override async Task<GetCompilersResponse> GetCompilers(Empty request, ServerCallContext context)
    {
        var response = await client.Compilers.GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            Compilers =
            {
                response?.Compilers?.Select(compilerInfo => new PublicCompilerInfo
                {
                    Id = compilerInfo.Id,
                    Name = compilerInfo.Name,
                    Style = !string.IsNullOrWhiteSpace(compilerInfo.Style)
                                ? compilerInfo.Style
                                : null,
                    Deprecated = compilerInfo.Deprecated ?? throw new("No valid deprecated flag")
                })
            }
        };
    }
}
