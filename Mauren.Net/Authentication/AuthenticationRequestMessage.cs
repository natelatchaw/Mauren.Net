using System;
using System.Net.Http;
using System.Net.Http.Headers;

using HttpRequestHeaders = Mauren.Net.Http.Headers.HttpRequestHeaders;

namespace Mauren.Net.Authentication
{
    public class AuthenticationRequestMessage : IAuthenticationRequestMessage
    {
        public HttpMethod Method { get; }
        public AuthenticationHeaderValue? Authorization => Credential.Authorization;
        public HttpRequestHeaders Headers { get; }
        public HttpRequestOptions Options { get; }
        public HttpContent Content => Credential.Content;
        public Uri RequestUri { get; }

        public ICredential Credential { get; set; }
        public IEndpoint Endpoint { get; }

        public AuthenticationRequestMessage(
            HttpMethod method,
            Uri requestUri,
            ICredential credential,
            HttpRequestHeaders? headers = default,
            HttpRequestOptions? options = default
        )
        {
            Method = method;
            Headers = headers ?? new();
            Options = options ?? new();
            RequestUri = requestUri;
            Credential = credential;

            Endpoint = null;
        }
    }
}
