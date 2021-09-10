using System.Text.Json.Serialization;

namespace GoogleDotNet.OAuth.Models
{
    public class TokenFromRefreshResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("grant_type")]
        public string GrantType { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }
    }
}
