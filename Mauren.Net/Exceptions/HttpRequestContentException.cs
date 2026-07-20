using System;
using System.Net.Http;

namespace Mauren.Net.Http
{
    public class HttpRequestContentException : HttpRequestException
    {
        public Uri Uri { get; }
        public HttpContent? Request { get; }
        public HttpContent? Response { get; }
        public HttpRequestContentException(Uri uri, HttpRequestException exception, HttpContent? requestContent, HttpContent? responseContent)
            : base(exception.Message, exception.InnerException, exception.StatusCode)
        {
            Uri = uri;
            Request = requestContent;
            Response = responseContent;
        }

        public override String Message => $"{base.Message} [{this.Uri}]";
    }
}
