using GoogleDotNet.OAuth.Attributes;

namespace GoogleDotNet.OAuth.Models
{
    /// <summary>
    /// OAuth 2.0 request for an access token as specified in http://tools.ietf.org/html/rfc6749#section-4.
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Gets or sets space-separated list of scopes as specified in http://tools.ietf.org/html/rfc6749#section-3.3.
        /// </summary>
        [QueryParameter("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the grant type. Sets either "authorization_code", "password", "client_credentials" or "refresh_token" or the
        /// absolute URI of the extension grant type.
        /// </summary>
        [QueryParameter("grant_type")]
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or gets the client ID.
        /// </summary>
        [QueryParameter("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        [QueryParameter("client_secret")]
        public string ClientSecret { get; set; }
    }
}
