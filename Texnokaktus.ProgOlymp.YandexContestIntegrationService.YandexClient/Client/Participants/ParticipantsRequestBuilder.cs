// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.Item;
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants
{
    /// <summary>
    /// Builds and executes requests for operations under \participants
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class ParticipantsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.participants.item collection</summary>
        /// <param name="position">participantId</param>
        /// <returns>A <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.Item.WithParticipantItemRequestBuilder"/></returns>
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.Item.WithParticipantItemRequestBuilder this[long position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("participantId", position);
                return new global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.Item.WithParticipantItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.ParticipantsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ParticipantsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/participants", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Participants.ParticipantsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ParticipantsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/participants", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
