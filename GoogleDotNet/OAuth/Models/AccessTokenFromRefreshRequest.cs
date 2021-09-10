using System.Text.Json.Serialization;

namespace GoogleDotNet.OAuth.Models
{
    public class AccessTokenFromRefreshRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("grant_type")]
        public string GrantType => "refresh_token";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
