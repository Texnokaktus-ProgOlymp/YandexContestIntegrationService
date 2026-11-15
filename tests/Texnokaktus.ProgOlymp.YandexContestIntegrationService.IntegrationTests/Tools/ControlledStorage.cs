using System.Collections.Concurrent;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using NSubstitute;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class ControlledStorage
{
    public IAmazonS3 Client { get; } = Substitute.For<IAmazonS3>();
    public ConcurrentQueue<(string Bucket, string Key, string Body)> Uploads { get; } = new();
    public bool FailUploads { get; set; }

    public ControlledStorage()
    {
        Client.Config.Returns(new AmazonS3Config { RegionEndpoint = RegionEndpoint.USEast1 });

        Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>())
              .Returns(async call =>
                   {
                       var request = call.Arg<PutObjectRequest>();
                       var cancellationToken = call.Arg<CancellationToken>();
                       using var reader = new StreamReader(request.InputStream, leaveOpen: true);

                       Uploads.Enqueue(
                           (request.BucketName, request.Key, await reader.ReadToEndAsync(cancellationToken))
                       );

                       if (FailUploads)
                           throw new AmazonS3Exception("Controlled archive failure");

                       return new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
                   }
               );
    }

    public void Reset()
    {
        Uploads.Clear();
        FailUploads = false;
        Client.ClearReceivedCalls();
    }
}
