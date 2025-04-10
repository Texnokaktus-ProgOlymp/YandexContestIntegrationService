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
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models;
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My
{
    /// <summary>
    /// Builds and executes requests for operations under \contests\neurips\{contestId}\submissions\my
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class MyRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public MyRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/contests/neurips/{contestId}/submissions/my{?onlyFinished*,pageNumber*,pageSize*,sortBy*,sortDesc*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public MyRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/contests/neurips/{contestId}/submissions/my{?onlyFinished*,pageNumber*,pageSize*,sortBy*,sortDesc*}", rawUrl)
        {
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt;
        /// </summary>
        /// <returns>A <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.NeuripsSubmissionsReportResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.NeuripsSubmissionsReportResponse?> GetAsync(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder.MyRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.NeuripsSubmissionsReportResponse> GetAsync(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder.MyRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.NeuripsSubmissionsReportResponse>(requestInfo, global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.NeuripsSubmissionsReportResponse.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt;
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder.MyRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder.MyRequestBuilderGetQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.MyRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Required scope: &lt;code&gt;submit&lt;/code&gt;
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class MyRequestBuilderGetQueryParameters 
        {
            /// <summary>onlyFinished</summary>
            [QueryParameter("onlyFinished")]
            public bool? OnlyFinished { get; set; }
            /// <summary>pageNumber</summary>
            [QueryParameter("pageNumber")]
            public int? PageNumber { get; set; }
            /// <summary>pageSize</summary>
            [QueryParameter("pageSize")]
            public int? PageSize { get; set; }
            /// <summary>sortBy</summary>
            [QueryParameter("sortBy")]
            public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.GetSortByQueryParameterType? SortBy { get; set; }
            /// <summary>sortDesc</summary>
            [QueryParameter("sortDesc")]
            public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Contests.Neurips.Item.Submissions.My.GetSortDescQueryParameterType? SortDesc { get; set; }
        }
    }
}
#pragma warning restore CS0618
