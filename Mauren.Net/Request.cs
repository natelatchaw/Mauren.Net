using Mauren.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

using HttpRequestHeaders = Mauren.Net.Http.Headers.HttpRequestHeaders;


namespace Mauren.Net
{
    internal class Request<T> : IHttpRequestMessage
    {
        public HttpMethod Method { get; protected set; }
        public HttpRequestHeaders Headers { get; protected set; }
        public HttpRequestOptions Options { get; protected set; }
        public HttpContent Content { get; protected set; }
        public Uri RequestUri { get; protected set; }
        public T? Body { get; protected set; }


        public Request(
            HttpMethod method,
            Uri requestUri,
            HttpRequestHeaders? requestHeaders = default,
            HttpRequestOptions? requestOptions = default,
            T? content = default,
            JsonSerializerOptions? serializerOptions = default,
            String mediaType = MediaTypeNames.Application.Json
        )
        {
            Method = method;
            RequestUri = requestUri;
            Headers = requestHeaders ?? new();
            Options = requestOptions ?? new();
            Content = JsonContent.Create<T?>(content, new(mediaType), serializerOptions);
        }
    }
}
