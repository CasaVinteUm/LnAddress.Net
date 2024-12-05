using System.Text.Json.Serialization;

namespace LnAddress.Net.Models.Responses;

public class LnUrlCallbackResponse(string pr)
{
    /// <summary>
    /// bech32-serialized lightning invoice
    /// </summary>
    [JsonPropertyName("pr")]
    public string Pr { get; } = pr;

    /// <summary>
    /// an empty array
    /// </summary>
    [JsonPropertyName("routes")]
    public string[] Routes { get; } = [];
}