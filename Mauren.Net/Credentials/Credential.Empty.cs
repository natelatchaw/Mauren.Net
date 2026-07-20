using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mauren.Net.Credentials
{
    public partial class EmptyCredential
    {

    }

    partial class EmptyCredential : ICredential
    {
        public AuthenticationHeaderValue? Authorization => new(String.Empty);

        public HttpMethod? Method => default;

        public Uri? RequestUri => default;

        public HttpContent Content => default;
    }
}
