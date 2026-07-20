using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Web;

namespace Mauren.Net.Test
{
    static partial class Helpers
    {
        public static String ToDebugString(this HttpRequestMessage requestMessage, JsonSerializerOptions? options = default, CancellationToken? cancellationToken = default, Boolean includeOverview = false)
        {
            StringBuilder builder = new();

            builder.AppendLine($"[REQUEST] @ {DateTimeOffset.Now}");
            // Method & Uri
            builder.AppendLine($"{requestMessage.Method} {requestMessage.RequestUri?.PathAndQuery}");
            // Host
            builder.AppendLine($"-- {nameof(requestMessage.RequestUri.Host)}: {requestMessage.RequestUri?.Host}");

            // Headers
            builder.AppendLine($"-- {nameof(requestMessage.Headers)}:");
            foreach (KeyValuePair<String, IEnumerable<String>> header in requestMessage.Headers.DefaultIfEmpty(new("None", [])))
                builder.AppendLine($"{header.Key} {String.Join(' ', header.Value)}");

            // Content
            builder.AppendLine($"-- {nameof(requestMessage.Content)}:");
            String content = (requestMessage.Content?.Headers.ContentType?.MediaType) switch
            {
                MediaTypeNames.Application.Json => requestMessage.GetJsonContent(options, cancellationToken),
                MediaTypeNames.Application.FormUrlEncoded => requestMessage.GetFormEncodedContent(cancellationToken),

                String value => $"The {value} Content-Type is not currently supported.",
                null => $"No Content-Type was found.",
            };
            builder.AppendLine($"{content}");

            if (includeOverview)
            {
                String requestInformation = JsonSerializer.Serialize(requestMessage, options);
                builder.AppendLine($"-- Overview:");
                builder.AppendLine($"{requestInformation}");
            }

            return builder.ToString();
        }

        public static String GetJsonContent(this HttpRequestMessage requestMessage, JsonSerializerOptions? options = default, CancellationToken? cancellationToken = default)
        {
            const String mediaType = MediaTypeNames.Application.Json;
            StringBuilder builder = new();

            if (requestMessage.Content?.Headers.ContentType is not MediaTypeHeaderValue mediaTypeHeaderValue)
                throw new InvalidOperationException($"The {nameof(HttpRequestMessage)}'s content type was not available.");
            if (mediaTypeHeaderValue.MediaType != mediaType)
                throw new InvalidOperationException($"The {nameof(HttpRequestMessage)}'s content type was not {mediaType}: found {mediaTypeHeaderValue.MediaType}");

            // Read the request message's content as a stream
            using Stream? stream = requestMessage.Content.ReadAsStream(cancellationToken ?? CancellationToken.None);
            // Initialize a stream reader from the stream
            using StreamReader reader = new(stream);

            String content = reader.ReadToEnd();

            builder.AppendLine(content);

            return builder.ToString();
        }

        public static String GetFormEncodedContent(this HttpRequestMessage requestMessage, CancellationToken? cancellationToken = default)
        {
            const String mediaType = MediaTypeNames.Application.FormUrlEncoded;
            StringBuilder builder = new();

            if (requestMessage.Content?.Headers.ContentType is not MediaTypeHeaderValue mediaTypeHeaderValue)
                throw new InvalidOperationException($"The {nameof(HttpRequestMessage)}'s content type was not available.");
            if (mediaTypeHeaderValue.MediaType != mediaType)
                throw new InvalidOperationException($"The {nameof(HttpRequestMessage)}'s content type was not {mediaType}: found {mediaTypeHeaderValue.MediaType}");

            // Read the request message's content as a stream
            using Stream? stream = requestMessage.Content.ReadAsStream(cancellationToken ?? CancellationToken.None);
            // Initialize a stream reader from the stream
            using StreamReader reader = new(stream);

            String content = reader.ReadToEnd();
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(content);
            Dictionary<String, String> parameters = queryParameters.AllKeys
                .ToDictionary((String? key) => key ?? String.Empty, (String? key) => String.Join(',', queryParameters.GetValues(key) ?? []));

            foreach ((String key, String value) in parameters)
                builder.AppendLine($"{key}: {value}");

            builder.AppendLine(content);

            return builder.ToString();
        }
    }
}
