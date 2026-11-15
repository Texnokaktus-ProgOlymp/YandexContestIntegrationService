using Microsoft.AspNetCore.Mvc.Testing;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[NonParallelizable]
public abstract class SetupBase
{
    protected const string SubmissionServiceName = "SubmissionService";
    protected const string ContestServiceName = "ContestDataService";
    protected static CustomWebApplicationFactory Factory => SetupGlobal.Factory;
    protected GrpcApi Api { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        Factory.Upstream.Reset();
        Factory.Storage.Reset();

        Api = new(
            Factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new("http://localhost"),
                    AllowAutoRedirect = false,
                    HandleCookies = false
                }
            )
        );
    }

    [TearDown]
    public void TearDown()
    {
        Api.Dispose();
        Factory.Upstream.Reset();
        Factory.Storage.Reset();
    }
}
