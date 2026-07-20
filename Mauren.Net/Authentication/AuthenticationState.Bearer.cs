using Mauren.Net.Exceptions;
using Mauren.Net.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Mauren.Net.Authentication
{
    /// <summary>
    /// Tracks the state of bearer token based authentication.
    /// </summary>
    /// 
    /// <typeparam name="TAuthenticationResponseContent">
    /// A class representing the response content received from an authentication request
    /// with the <c>Bearer</c> scheme.
    /// </typeparam>
    public class BearerAuthenticationState<TAuthenticationResponseContent> : AuthenticationState
        where TAuthenticationResponseContent : IBearerAuthenticationResponseContent
    {
        public BearerAuthenticationState() { }

        #region overrides

        protected override String Scheme { get; } = "Bearer";


        /// <remarks>
        /// Will <see langword="throw"/> an <see cref="InvalidAuthenticationValueException"/> if the access token has expired or is missing.
        /// </remarks>
        /// 
        /// <inheritdoc/>
        protected override String? Parameter
        {
            get
            {
                // If the access token has expired
                if (Access.Expiration is DateTimeOffset expiration && DateTimeOffset.UtcNow > expiration)
                    throw new ExpiredAuthenticationValueException("Access Token", expiration);
                // If the access token is not available
                if (Access.Value is not String token)
                    throw new MissingAuthenticationValueException("Access Token");

                // Return the access token
                return token;
            }
        }

        /// <inheritdoc/>
        public override HttpRequestMessage? Request
        {
            get
            {
                // Determine if the access token is expired
                Boolean isExpired = DateTimeOffset.UtcNow >= Access.Expiration.GetValueOrDefault(DateTimeOffset.MinValue);

                // If the HTTP method is null
                if (Credential?.Method is not HttpMethod method)
                    // Throw exception
                    throw new InvalidOperationException($"No {nameof(Method)} was provided");

                // If the request URI is null
                if (Credential?.RequestUri is not Uri requestUri)
                    // Throw exception
                    throw new InvalidOperationException($"No {nameof(RequestUri)} was provided");

                // If the access token has yet to expire
                if (isExpired is false)
                    // Return default, no authentication required at this time
                    return default;

                // If a refresh token is available
                else if (Refresh.RefreshToken is not null)
                    // Return a request with the refresh credential
                    return new AuthenticationRequestMessage(method, requestUri, Refresh, default, default).AsRequestMessage();

                // Otherwise
                else
                    // Return a request with the main credential
                    return new AuthenticationRequestMessage(method, requestUri, Credential, default, default).AsRequestMessage();
            }
        }

        /// <inheritdoc/>
        public override HttpResponseMessage? Response
        {
            set
            {
                // If the response was null
                if (value is not HttpResponseMessage response)
                    // Throw exception
                    throw new InvalidOperationException($"No {nameof(HttpResponseMessage)} was provided.");

                try
                {
                    // If the response's status code indicates failure, throw exception
                    response.EnsureSuccessStatusCode();

                    // Read the response's content as a stream
                    using Stream stream = response.Content.ReadAsStream();

                    // Deserialize the response content stream
                    TAuthenticationResponseContent? content = JsonSerializer.Deserialize<TAuthenticationResponseContent>(stream, options: default);

                    // Process the received response's content
                    this.ProcessResponseContent(content);
                }
                // If the response message indicates failure
                catch (HttpRequestException exception)
                {
                    // Read the response content as a stream
                    using Stream stream = response.Content.ReadAsStream();

                    // Create a stream reader from the stream
                    using StreamReader reader = new StreamReader(stream);

                    // Read the stream's content to a string
                    String content = reader.ReadToEnd();

                    // Throw an exception from the read content
                    throw new Exception(content, exception);
                }
            }
        }

        /// <summary>
        /// Processes the <typeparamref name="TAuthenticationResponseContent"/> received
        /// from the remote authentication endpoint.
        /// </summary>
        /// 
        /// <param name="content">
        /// The <typeparamref name="TAuthenticationResponseContent"/> instance deserialized from
        /// the <see cref="HttpResponseMessage.Content"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown when the received <paramref name="content"/> value is null.
        /// </exception>
        /// 
        /// <remarks>
        /// Override this method to provide custom handling of
        /// received <typeparamref name="TAuthenticationResponseContent"/>.
        /// </remarks>
        protected virtual void ProcessResponseContent(TAuthenticationResponseContent? content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content), $"Received response data was null.");

            // Set the access token's value to the content's access token
            Access.Value = content.AccessToken;
            // Set the access token's expiration to the calculated timestamp
            Access.Expiration = DateTimeOffset.UtcNow.AddSeconds(content.ExpiresIn);
            // Set the refresh credential
            Refresh = new RefreshCredential
            {
                Method = Credential.Method,
                RequestUri = Credential.RequestUri,
                AccessToken = content.AccessToken,
                RefreshToken = content.RefreshToken,
            };
        }

        /// <remarks>
        /// Clears the refresh token and resets the access token's expiration timestamp.
        /// </remarks>
        /// <inheritdoc/>
        public override Boolean TryReset(HttpStatusCode? statusCode = null)
        {
            String accessToken = Refresh.AccessToken;
            Refresh = new RefreshCredential
            {
                Method = Credential.Method,
                RequestUri = Credential.RequestUri,
                AccessToken = accessToken,
                RefreshToken = default,
            };
            Access.Expiration = null;
            return statusCode switch { _ => true };
        }

        #endregion


        #region implementation

        /// <summary>
        /// Contains the access token's data.
        /// </summary>
        protected class Token
        {
            /// <summary>
            /// The access token's <see cref="String">value</see>.
            /// </summary>
            public String? Value { get; set; }

            /// <summary>
            /// The access token's <see cref="DateTimeOffset">expiration</see>, if any.
            /// </summary>
            public DateTimeOffset? Expiration { get; set; }
        }

        /// <summary>
        /// The token containing the access token string, as well
        /// as metadata about the token.
        /// </summary>
        protected Token Access { get; set; } = new();

        /// <summary>
        /// A <see cref="ICredential"/> containing refresh data.
        /// </summary>
        protected RefreshCredential Refresh { get; set; } = new();

        /// <summary>
        /// The primary Authorization Grant request used for initial authorization.
        /// </summary>
        //protected IAuthenticationRequestMessage PrimaryRequest => new AuthenticationRequestMessage<TCredential>(this.Method, this.RequestUri, this.Credential);

        /// <summary>
        /// The refresh Authentication Grant request used to refresh the access token when it expires.
        /// </summary>
        //protected AuthenticationRequestMessage<RefreshCredential> RefreshRequest { get; set; }

        #endregion
    }
}
