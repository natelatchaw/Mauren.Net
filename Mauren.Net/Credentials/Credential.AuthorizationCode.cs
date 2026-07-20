using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mauren.Net.Authentication
{
    public partial class AuthorizationCodeCredential
    {
        public HttpMethod? Method { get; set; }
        public Uri? RequestUri { get; set; }

        public String GrantType { get; set; } = "authorization_code";
        public String Code { get; set; }
        public String ClientID { get; set; }
        public String ClientSecret { get; set; }
        public Uri RedirectUri { get; set; }

        private IEnumerable<KeyValuePair<String, String?>> Data => new Dictionary<String, String?>
        {
            { "grant_type", GrantType },
            { "code", Code },
            { "client_id", ClientID },
            { "client_secret", ClientSecret },
            { "redirect_uri", RedirectUri.AbsoluteUri },
        };
    }
    
    partial class AuthorizationCodeCredential : ICredential
    {
        HttpMethod? ICredential.Method => Method;
        Uri? ICredential.RequestUri => RequestUri;
        AuthenticationHeaderValue? ICredential.Authorization => default;
        HttpContent ICredential.Content => new FormUrlEncodedContent(Data);
    }
}
