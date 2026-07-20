using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Mauren.Net.Test
{
    public static class HttpRequestMessageExtensions
    {
        [Obsolete($"Use {nameof(Helpers.ToDebugString)}", error: true)]
        public static String PrintRequest(this HttpRequestMessage message)
        {
            StringBuilder builder = new();

            builder.AppendLine($"{message.Method} {message.RequestUri?.PathAndQuery}");
            builder.AppendLine($"Host: {message.RequestUri?.Host}");

            foreach (KeyValuePair<String, IEnumerable<String>> header in message.Headers)
                builder.AppendLine($"{header.Key}: {String.Join(' ', header.Value)}");

            return builder.ToString();
        }
    }
}
