using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mauren.Net.Authentication
{
    public partial class ResourceOwnerPasswordCredential
    {
        String GrantType { get; } = "password";
        public String Username { get; set; }
        public String Password { get; set; }

        private IEnumerable<KeyValuePair<String, String>> Data => new Dictionary<String, String>
        {
            { "grant_type", GrantType },
            { "username", Username },
            { "password", Password },
        };
    }
    
    partial class ResourceOwnerPasswordCredential : ICredential
    {
        HttpMethod? ICredential.Method => default;
        Uri? ICredential.RequestUri => default;
        AuthenticationHeaderValue? ICredential.Authorization => default;
        HttpContent ICredential.Content => new FormUrlEncodedContent(Data);
    }
}
