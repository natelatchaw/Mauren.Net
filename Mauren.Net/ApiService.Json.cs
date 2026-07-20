using Mauren.Net.Authentication;
using Mauren.Net.Http;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net
{
    public abstract partial class JsonApiService<TCredential, TAuthenticationState> : ApiService<TCredential, TAuthenticationState>
        where TCredential : class, ICredential
        where TAuthenticationState : IAuthenticationState, new()
    {
        /// <summary>
        /// Options for serialization of JSON payloads.
        /// </summary>
        public abstract JsonSerializerOptions? SerializerOptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiService{TCredential, TAuthenticationState}"/> class
        /// with an <see cref="System.Net.Http.HttpClient"/> and <typeparamref name="TCredential"/>.
        /// </summary>
        /// 
        /// <param name="httpClient">
        /// The <see cref="System.Net.Http.HttpClient"/> used to send HTTP requests.
        /// </param>
        /// 
        /// <param name="credential">
        /// The <typeparamref name="TCredential"/> data used for authentication.
        /// </param>
        protected JsonApiService(HttpClient httpClient, TCredential credential) : base(httpClient, credential) { }

        public virtual async Task<TValue?> SendAsync<TValue>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            // Send the request message via the base class's SendAsync method
            HttpResponseMessage responseMessage = await base.SendAsync(requestMessage, cancellationToken);
            // Read the response's content as a JSON object
            TValue? value = await responseMessage.Content.ReadFromJsonAsync<TValue>(SerializerOptions, cancellationToken);
            // Return the deserialized value
            return value;
        }
    }

    public static class JsonApiServiceExtensions
    {
        public static async Task<TValue?> SendAsync<TValue, TCredential, TAuthenticationState>(this JsonApiService<TCredential, TAuthenticationState> apiService, IHttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
            where TCredential : class, ICredential
            where TAuthenticationState: IAuthenticationState, new()
        {
            HttpRequestMessage requestMessage = httpRequestMessage.AsRequestMessage();
            return await apiService.SendAsync<TValue>(requestMessage, cancellationToken);
        }
    }
}
