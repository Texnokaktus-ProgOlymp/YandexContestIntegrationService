using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

/// <summary>
/// Implementation of the gRPC CompilerService for the Yandex Contest Integration Service.
/// This service provides functionality to interact with the compiler information
/// for the Yandex Contest system.
/// </summary>
public class CompilerServiceImpl(ContestClient client) : CompilerService.CompilerServiceBase
{
    /// <summary>
    /// Retrieves the list of available compilers along with their corresponding details.
    /// </summary>
    /// <param name="request">The request object, which is expected to be empty.</param>
    /// <param name="context">The server call context associated with the gRPC call.</param>
    /// <returns>A task representing the asynchronous operation, which resolves to a <see cref="GetCompilersResponse"/> containing the list of compilers.</returns>
    /// <exception cref="Exception">Thrown when an invalid deprecated flag is encountered in the compiler data.</exception>
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
