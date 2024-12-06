using System.Text.Json.Serialization;

namespace LnAddress.Net.Models.Responses;

public class LnAddressWellKnownResponse(string callback, long minSendableMillisats, long maxSendableMillisats, string? tag = null)
{
    /// <summary>
    /// The URL from LN SERVICE which will accept the pay request parameters
    /// </summary>
    [JsonPropertyName("callback")]
    public string Callback { get; } = callback;

    /// <summary>
    /// Max millisatoshi amount LN SERVICE is willing to receive
    /// </summary>
    [JsonPropertyName("maxSendable")]
    public long MaxSendable { get; } = maxSendableMillisats;

    /// <summary>
    /// Min millisatoshi amount LN SERVICE is willing to receive, can not be less than 1 or more than `maxSendable`
    /// </summary>
    [JsonPropertyName("minSendable")]
    public long MinSendable { get; } = minSendableMillisats;

    /// <summary>
    /// Metadata json which must be presented as raw string here, this is required to pass signature verification
    /// at a later step
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Metadata { get; private set; }

    /// <summary>
    /// Size of comment allowed in the invoice
    /// Null if no comment allowed
    /// </summary>
    [JsonPropertyName("commentAllowed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CommentAllowed { get; private set; }

    /// <summary>
    /// Type of LNURL
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; } = tag ?? "payRequest";

    public LnAddressWellKnownResponse WithMetadata(string? metadata)
    {
        Metadata = metadata;
        return this;
    }

    public LnAddressWellKnownResponse WithCommentAllowed(int? commentAllowed)
    {
        CommentAllowed = commentAllowed > 0 ? commentAllowed : null;
        return this;
    }
}