using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class GrpcApi : IDisposable
{
    private readonly GrpcChannel channel;
    private readonly CallInvoker invoker;

    public GrpcApi(HttpClient httpClient)
    {
        channel = GrpcChannel.ForAddress(
            httpClient.BaseAddress!,
            new GrpcChannelOptions
            {
                HttpClient = httpClient,
                DisposeHttpClient = true
            }
        );

        invoker = channel.CreateCallInvoker();
    }

    public AsyncUnaryCall<TResponse> Unary<TRequest, TResponse>(
        string service,
        string method,
        TRequest request,
        MessageParser<TResponse> parser,
        CancellationToken cancellationToken = default
    )
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new() =>
        invoker.AsyncUnaryCall(
            new Method<TRequest, TResponse>(
                MethodType.Unary,
                service,
                method,
                Marshaller(new MessageParser<TRequest>(() => new())),
                Marshaller(parser)
            ),
            null,
            new CallOptions(cancellationToken: cancellationToken),
            request
        );

    public AsyncServerStreamingCall<TResponse> Stream<TRequest, TResponse>(
        string service,
        string method,
        TRequest request,
        MessageParser<TResponse> parser
    )
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new() =>
        invoker.AsyncServerStreamingCall(
            new Method<TRequest, TResponse>(
                MethodType.ServerStreaming,
                service,
                method,
                Marshaller(new MessageParser<TRequest>(() => new())),
                Marshaller(parser)
            ),
            null,
            new CallOptions(),
            request
        );

    private static Marshaller<T> Marshaller<T>(MessageParser<T> parser) where T : class, IMessage<T> =>
        Marshallers.Create<T>(message => message.ToByteArray(), parser.ParseFrom);

    public void Dispose() => channel.Dispose();
}
