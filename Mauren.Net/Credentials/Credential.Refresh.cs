using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mauren.Net.Authentication
{
    public partial class RefreshCredential
    {
        public HttpMethod? Method { get; set; }
        public Uri? RequestUri { get; set; }

        String GrantType { get; set; } = "refresh_token";
        public String AccessToken { get; set; }
        public String RefreshToken { get; set; }

        private IEnumerable<KeyValuePair<String, String>> Data => new Dictionary<String, String>()
        {
            { "grant_type", GrantType },
            { "refresh_token", RefreshToken }
        };
    }
    
    partial class RefreshCredential : ICredential
    {
        HttpMethod? ICredential.Method => Method;
        Uri? ICredential.RequestUri => RequestUri;
        AuthenticationHeaderValue? ICredential.Authorization => new("Bearer", AccessToken);
        HttpContent ICredential.Content => new FormUrlEncodedContent(Data);
    }
}
