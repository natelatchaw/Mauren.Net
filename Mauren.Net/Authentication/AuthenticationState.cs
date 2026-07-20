using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net.Authentication
{
    /// <summary>
    /// A contract defining authentication behavior for receiving credentials.
    /// </summary>
    public interface IAuthenticationState
    {
        /// <summary>
        /// Sets the <see cref="ICredential">credential</see> to be used for authentication.
        /// </summary>
        public ICredential? Credential { set; }

        public AuthenticationHeaderValue? Header { get; }

        /// <summary>
        /// Provides a <see cref="HttpRequestMessage">request</see> to be transmitted to the
        /// remote authentication endpoint.
        /// 
        /// Providing <see langword="default"/> indicates that authentication is not required.
        /// </summary>
        /// 
        /// <remarks>
        /// The <see cref="HttpRequestMessage">request</see> should contain all data needed to
        /// receive an <see cref="HttpResponseMessage">response</see> containing authentication
        /// data from the remote endpoint.
        /// </remarks>
        public HttpRequestMessage? Request { get; }

        /// <summary>
        /// Supplies a <see cref="HttpResponseMessage">response</see> received from the 
        /// remote authentication endpoint to be processed.
        /// </summary>
        /// 
        /// <remarks>
        /// The <see cref="HttpResponseMessage">response</see> should be parsed for credentials
        /// provided by the remote endpoint, and internal state of this instance updated to reflect
        /// the received credentials.
        /// </remarks>
        public HttpResponseMessage? Response { set; }
    }

    /// <summary>
    /// Tracks the state of authorization.
    /// </summary>
    /// <remarks>
    /// Inherit from this to implement a custom authorization scheme.
    /// </remarks>
    public abstract class AuthenticationState : IAuthenticationState
    {
        /// <summary>
        /// The <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication#authentication_schemes">authentication scheme</see> value
        /// to be used in the <see cref="AuthenticationHeaderValue"/>.
        /// </summary>
        protected abstract String Scheme { get; }

        /// <summary>
        /// The parameter value
        /// to be used in the <see cref="AuthenticationHeaderValue"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// A <see langword="default"/> value returned from this member's getter
        /// will cause the <see cref="Header"/> value to be returned, albeit with a <see langword="default"/>
        /// <see cref="AuthenticationHeaderValue.Parameter"/> value.
        /// 
        /// An <see cref="Exception"/> thrown from this member's getter
        /// will cause the <see cref="Header"/> value itself to return <see langword="default"/>.
        /// </remarks>
        protected abstract String? Parameter { get; }

        /// <summary>
        /// The Authorization Header used to authenticate HTTP requests.
        /// </summary>
        public AuthenticationHeaderValue? Header
        {
            get
            {
                try
                {
                    return new(Scheme, Parameter);
                }
                // If an exception occurred retrieving the scheme or parameter
                catch
                {
                    return default;
                }
            }
        }

        /// <inheritdoc/>
        public virtual HttpMethod? Method { protected get; set; }

        /// <inheritdoc/>
        public virtual Uri? RequestUri { protected get; set; }

        /// <inheritdoc/>
        public virtual ICredential? Credential { protected get; set; }

        /// <summary>
        /// Provides a <see cref="HttpRequestMessage">request</see> to be transmitted to the
        /// remote authentication endpoint.
        /// 
        /// Providing <see langword="default"/> indicates that authentication is not required.
        /// </summary>
        /// 
        /// <remarks>
        /// The <see cref="HttpRequestMessage">request</see> should contain all data needed to
        /// receive an <see cref="HttpResponseMessage">response</see> containing authentication
        /// data from the remote endpoint.
        /// </remarks>
        public abstract HttpRequestMessage? Request { get; }

        /// <summary>
        /// Supplies a <see cref="HttpResponseMessage">response</see> received from the 
        /// remote authentication endpoint to be processed.
        /// </summary>
        /// 
        /// <remarks>
        /// The <see cref="HttpResponseMessage">response</see> should be parsed for credentials
        /// provided by the remote endpoint, and internal state of this instance updated to reflect
        /// the received credentials.
        /// </remarks>
        public abstract HttpResponseMessage? Response { set; }

        /// <summary>
        /// Attempts to reset the internal state of the authentication process.
        /// </summary>
        /// 
        /// <remarks>
        /// When overriding in a derived class, a successful reset should clear
        /// any stored credentials and state data.
        /// </remarks>
        /// 
        /// <param name="statusCode">
        /// The <see cref="HttpStatusCode"/> that was received leading to a reset
        /// being requested.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Boolean"/> indicating whether the reset was successful
        /// and the request should be retried.
        /// </returns>
        public abstract Boolean TryReset(HttpStatusCode? statusCode = default);
    }
}

namespace System.Net.Http
{
    public static class AuthenticationStateExtensions
    {
        /// <summary>
        /// Updates the <typeparamref name="TAuthenticationState"/> by transmitting and receiving
        /// authentication data via the provided <paramref name="client"/>.
        /// </summary>
        /// 
        /// <typeparam name="TAuthenticationState">
        /// The type of <see cref="Mauren.Net.Authentication.AuthenticationState"/> to update.
        /// </typeparam>
        /// 
        /// <param name="client">
        /// The <see cref="HttpClient"/> to use to update the provided <paramref name="state"/>.
        /// </param>
        /// 
        /// <param name="state">
        /// The <see cref="Mauren.Net.Authentication.AuthenticationState"/> to update.
        /// </param>
        /// 
        /// <param name="cancellationToken">
        /// The cancellation token to cancel operation.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Task"/> indicating completion.
        /// </returns>
        public static async Task UpdateAsync<TAuthenticationState>(this HttpClient client, TAuthenticationState state, CancellationToken cancellationToken = default, Func<HttpResponseMessage, CancellationToken, Task>? validate = default)
            where TAuthenticationState : Mauren.Net.Authentication.IAuthenticationState
        {
            // If the authentication state did not provide a request, return as no authentication is required
            if (state.Request is not HttpRequestMessage request) return;

            // Send the authentication request
            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            if (validate is Func<HttpResponseMessage, CancellationToken, Task> validateResponse)
            {
                await validateResponse.Invoke(response, cancellationToken);
            }
            else
            {
                // Ensure the response contains a successful status code
                response.EnsureSuccessStatusCode();
            }

            // Supply the response to the authentication state
            state.Response = response;
        }
    }
}