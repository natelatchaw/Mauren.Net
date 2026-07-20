using System;
using System.Net.Http;
using System.Text.Json;

namespace Mauren.Text.Json
{
    public class JsonRequestContentException : JsonException
    {
        public Uri Uri { get; }
        public HttpContent? Request { get; }
        public HttpContent? Response { get; }
        public JsonRequestContentException(Uri uri, JsonException exception, HttpContent? requestContent, HttpContent? responseContent)
            : base(exception.Message, exception.Path, exception.LineNumber, exception.BytePositionInLine, exception)
        {
            Uri = uri;
            Request = requestContent;
            Response = responseContent;
        }
    }
}
