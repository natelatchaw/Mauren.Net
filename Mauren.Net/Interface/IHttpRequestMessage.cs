using System.Collections.Generic;
using System.Net.Http;

namespace Mauren.Net.Http
{
    /// <summary>
    /// Defines the contract for objects representing a HTTP request message.
    /// </summary>
    public interface IHttpRequestMessage
    {
        /// <summary>
        /// Gets the HTTP method used by the HTTP request message.
        /// </summary>
        System.Net.Http.HttpMethod Method { get; }

        /// <summary>
        /// Gets the collection of HTTP request headers.
        /// </summary>
        Mauren.Net.Http.Headers.HttpRequestHeaders Headers { get; }

        /// <summary>
        /// Gets the collection of options to configure the HTTP request.
        /// </summary>
        System.Net.Http.HttpRequestOptions Options { get; }

        /// <summary>
        /// Gets the contents of the HTTP message.
        /// </summary>
        System.Net.Http.HttpContent Content { get; }

        /// <summary>
        /// Gets the <see cref="Uri"/> used for the HTTP request.
        /// </summary>
        System.Uri RequestUri { get; }
    }
}

namespace Mauren.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static System.Net.Http.HttpRequestMessage AsRequestMessage(this IHttpRequestMessage message)
        {
            // Initialize the HttpRequestMessage with the HttpMethod and Uri
            System.Net.Http.HttpRequestMessage httpRequestMessage = new(message.Method, message.RequestUri);

            // Copy the headers to the request message
            foreach (KeyValuePair<System.String, IEnumerable<System.String>> header in message.Headers)
                httpRequestMessage.Headers.Add(header.Key, header.Value);

            // Copy the options to the request message
            foreach (KeyValuePair<System.String, System.Object?> option in message.Options)
                httpRequestMessage.Options.TryAdd(option.Key, option.Value);

            // Copy the content to the request message
            httpRequestMessage.Content = message.Content;

            return httpRequestMessage;
        }
    }
}
