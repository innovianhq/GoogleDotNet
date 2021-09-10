using System;

namespace GoogleDotNet.OAuth
{
    public interface IOAuthHandler
    {
        /// <summary>
        /// The latest access token procured.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Event fired when the token is updated.
        /// </summary>
        public event EventHandler<string> OnAccessTokenUpdated;
    }
}
