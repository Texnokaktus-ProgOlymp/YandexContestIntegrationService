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

    public ConcurrentQueue<(string Bucket, string Key, byte[] Body, string? ContentType, string? ContentDisposition)>
        Uploads { get; } = new();

    public bool FailUploads { get; set; }
    public Func<CancellationToken, Task>? BeforeUploadComplete { get; set; }

    public ControlledStorage()
    {
        Client.Config.Returns(new AmazonS3Config { RegionEndpoint = RegionEndpoint.USEast1 });

        Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>())
              .Returns(async call =>
                   {
                       var request = call.Arg<PutObjectRequest>();
                       var cancellationToken = call.Arg<CancellationToken>();
                       using var buffer = new MemoryStream();
                       await request.InputStream.CopyToAsync(buffer, cancellationToken);

                       Uploads.Enqueue(
                           (request.BucketName, request.Key, buffer.ToArray(), request.ContentType,
                            request.Headers.ContentDisposition)
                       );

                       if (BeforeUploadComplete is { } complete) await complete(cancellationToken);

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
        BeforeUploadComplete = null;
        Client.ClearReceivedCalls();
    }
}
