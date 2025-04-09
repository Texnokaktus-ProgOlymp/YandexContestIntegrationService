// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item.Rejudge;
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item
{
    /// <summary>
    /// Builds and executes requests for operations under \submissions\{submissionId}
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class WithSubmissionItemRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The rejudge property</summary>
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item.Rejudge.RejudgeRequestBuilder Rejudge
        {
            get => new global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item.Rejudge.RejudgeRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item.WithSubmissionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithSubmissionItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/submissions/{submissionId}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Submissions.Item.WithSubmissionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithSubmissionItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/submissions/{submissionId}", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
