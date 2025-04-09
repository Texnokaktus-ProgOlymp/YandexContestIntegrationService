// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class ContestStandingsRow : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The participantInfo property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ParticipantInfo? ParticipantInfo { get; set; }
#nullable restore
#else
        public global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ParticipantInfo ParticipantInfo { get; set; }
#endif
        /// <summary>The placeFrom property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<int?>? PlaceFrom { get; set; }
#nullable restore
#else
        public List<int?> PlaceFrom { get; set; }
#endif
        /// <summary>The placeTo property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<int?>? PlaceTo { get; set; }
#nullable restore
#else
        public List<int?> PlaceTo { get; set; }
#endif
        /// <summary>The problemResults property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ProblemResult>? ProblemResults { get; set; }
#nullable restore
#else
        public List<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ProblemResult> ProblemResults { get; set; }
#endif
        /// <summary>The score property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Score { get; set; }
#nullable restore
#else
        public string Score { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ContestStandingsRow"/> and sets the default values.
        /// </summary>
        public ContestStandingsRow()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ContestStandingsRow"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ContestStandingsRow CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ContestStandingsRow();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "participantInfo", n => { ParticipantInfo = n.GetObjectValue<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ParticipantInfo>(global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ParticipantInfo.CreateFromDiscriminatorValue); } },
                { "placeFrom", n => { PlaceFrom = n.GetCollectionOfPrimitiveValues<int?>()?.AsList(); } },
                { "placeTo", n => { PlaceTo = n.GetCollectionOfPrimitiveValues<int?>()?.AsList(); } },
                { "problemResults", n => { ProblemResults = n.GetCollectionOfObjectValues<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ProblemResult>(global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ProblemResult.CreateFromDiscriminatorValue)?.AsList(); } },
                { "score", n => { Score = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ParticipantInfo>("participantInfo", ParticipantInfo);
            writer.WriteCollectionOfPrimitiveValues<int?>("placeFrom", PlaceFrom);
            writer.WriteCollectionOfPrimitiveValues<int?>("placeTo", PlaceTo);
            writer.WriteCollectionOfObjectValues<global::Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.Models.ProblemResult>("problemResults", ProblemResults);
            writer.WriteStringValue("score", Score);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
