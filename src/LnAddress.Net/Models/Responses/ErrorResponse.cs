using System.Text.Json.Serialization;

namespace LnAddress.Net.Models.Responses;

public class ErrorResponse(string reason)
{
    [JsonPropertyName("status")]
    public string Status { get; } = "ERROR";

    [JsonPropertyName("reason")]
    public string Reason { get; } = reason;
}