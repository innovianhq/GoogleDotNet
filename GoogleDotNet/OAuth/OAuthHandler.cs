using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GoogleDotNet.OAuth.Models;

namespace GoogleDotNet.OAuth
{
public class OAuthHandler : IOAuthHandler
    {
        private readonly Timer _renewalTimer;

        private readonly string _code;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        /// <summary>
        /// The value of the refresh token.
        /// </summary>
        private string _refreshToken { get; set; }

        /// <summary>
        /// The latest access token procured.
        /// </summary>
        public string AccessToken { get; private set; } = string.Empty;
        
        private OAuthHandler(string code, string clientId, string clientSecret, string redirectUri)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _code = code;
                
            _renewalTimer = new Timer(RenewalTimerCallback);
        }

        private OAuthHandler(string refreshToken, string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _refreshToken = refreshToken;

            _renewalTimer = new Timer(RenewalTimerCallback);
        }

        public event EventHandler<string> OnAccessTokenUpdated;

        /// <summary>
        /// Factory method to create the handler from a code token.
        /// </summary>
        /// <param name="code">The authorization code retrieved from an OAuth verification workflow.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="clientSecret">The OAuth client secret.</param>
        /// <param name="redirectUri">The redirect URI registered in the OAuth client.</param>
        /// <returns></returns>
        public static async Task<OAuthHandler> CreateFactoryWithCode(string code, string clientId, string clientSecret, string redirectUri)
        {
            var handler = new OAuthHandler(code, clientId, clientSecret, redirectUri);
            await handler.GetTokenFromCode();

            return handler;
        }

        /// <summary>
        /// Factory method to create the handler from a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token used to provision additional tokens.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="clientSecret">The OAuth client secret.</param>
        /// <param name="redirectUri">The redirect URI registered in the OAuth client.</param>
        /// <returns></returns>
        public static async Task<OAuthHandler> CreateFactoryWithRefreshToken(string refreshToken, string clientId, string clientSecret)
        {
            var handler = new OAuthHandler(refreshToken, clientId, clientSecret);
            await handler.GetTokenFromRefresh();

            return handler;
        }

        /// <summary>
        /// Triggers the call to get another token from the saved refresh token.
        /// </summary>
        /// <param name="state"></param>
        private async void RenewalTimerCallback(object? state)
        {
            await GetTokenFromRefresh();
            OnAccessTokenUpdated?.Invoke(this, AccessToken);
        }

                /// <summary>
        /// Used to procure a new access token from a given refresh token.
        /// </summary>
        /// <returns></returns>
        private async Task GetTokenFromRefresh()
        {
            using var httpClient = new HttpClient();

            //Create the request
            var requestUri = new Uri("https://www.googleapis.com/oauth2/v4/token");
            var jsonAuthObj = JsonSerializer.Serialize(new AccessTokenFromRefreshRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RefreshToken = _refreshToken
            });
            var contentBody = new StringContent(jsonAuthObj, Encoding.UTF8, "application/json");

            var resp = await httpClient.PostAsync(requestUri, contentBody);

            //Parse the response
            var deserializedResp = JsonSerializer.Deserialize<TokenFromRefreshResponse>(await resp.Content.ReadAsStringAsync());

            if (resp.StatusCode != HttpStatusCode.OK)
                throw new AuthenticationException(
                    "Unable to retrieve authorization token as the refresh token was not accepted");
            
            //Update the renewal timer
            var expiresIn = deserializedResp != null
                ? TimeSpan.FromSeconds(deserializedResp.ExpiresIn - 100)
                : TimeSpan.FromMinutes(30) - TimeSpan.FromSeconds(100);
            _renewalTimer.Change(expiresIn, Timeout.InfiniteTimeSpan);

            //Update the access token
            AccessToken = deserializedResp.AccessToken;
        }

        /// <summary>
        /// Used to procure a new access token from a given code (retrieved through an OAuth verification workflow).
        /// </summary>
        /// <returns></returns>
        private async Task GetTokenFromCode()
        {
            using var httpClient = new HttpClient();
            
            //Create the request
            var requestUri = new Uri("https://oauth2.googleapis.com/token");
            var jsonRefObj = JsonSerializer.Serialize(new AccessTokenFromCodeRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Code = _code,
                RedirectUri = _redirectUri
            });
            var contentBody = new StringContent(jsonRefObj, Encoding.UTF8, "application/json");
            
            var resp = await httpClient.PostAsync(requestUri, contentBody);

            //Parse the response
            var deserializedResp =
                JsonSerializer.Deserialize<TokenFromCodeResponse>(await resp.Content.ReadAsStringAsync());

            //Set the refresh token
            _refreshToken = deserializedResp.RefreshToken;

            //Update the renewal timer
            _renewalTimer.Change(TimeSpan.FromSeconds(deserializedResp.ExpiresIn - 100), Timeout.InfiniteTimeSpan);

            //Update the access token
            AccessToken = deserializedResp.AccessToken;
        }
    }
}
