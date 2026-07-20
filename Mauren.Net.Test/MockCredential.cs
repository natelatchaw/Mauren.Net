using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Mauren.Net.Test
{
    internal class MockCredential : ICredential
    {
        public HttpMethod? Method => HttpMethod.Post;
        public Uri? RequestUri => new Uri("https://localhost/auth/token");
        public AuthenticationHeaderValue? Authorization { get; set; }
        public HttpContent Content => JsonContent.Create<Object>(new());
    }
}
