using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Mauren.Net.Authentication
{
    public struct EndpointMetadata
    {
        public HttpMethod Method { get; set; }
        public Uri RequestUri { get; set; }
    }
}
