// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement
{
    /// <summary>
    /// Builds and executes requests for operations under \contests\{contestId}\problems\{alias}\statement
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class StatementRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public StatementRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/contests/{contestId}/problems/{alias}/statement{?locale*,type*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public StatementRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/contests/{contestId}/problems/{alias}/statement{?locale*,type*}", rawUrl)
        {
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt; or &lt;code&gt;manage&lt;/code&gt;
        /// </summary>
        /// <returns>A <see cref="Stream"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<Stream?> GetAsync(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder.StatementRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<Stream> GetAsync(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder.StatementRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendPrimitiveAsync<Stream>(requestInfo, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt; or &lt;code&gt;manage&lt;/code&gt;
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder.StatementRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder.StatementRequestBuilderGetQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/octet-stream");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.StatementRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt; or &lt;code&gt;manage&lt;/code&gt;
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class StatementRequestBuilderGetQueryParameters 
        {
            /// <summary>locale</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("locale")]
            public string? Locale { get; set; }
#nullable restore
#else
            [QueryParameter("locale")]
            public string Locale { get; set; }
#endif
            /// <summary>type</summary>
            [QueryParameter("type")]
            public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Item.Problems.Item.Statement.GetTypeQueryParameterType? Type { get; set; }
        }
    }
}
#pragma warning restore CS0618
