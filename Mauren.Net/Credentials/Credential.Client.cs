using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mauren.Net.Authentication
{
    public partial class ClientCredential
    {
        [Required]
        public String Method { get; set; }
        [Required]
        public Uri RequestUri { get; set; }

        public String GrantType { get; set; } = "client_credentials";
        public String ID { get; set; }
        public String Secret { get; set; }
        public String Scope { get; set; }

        private IEnumerable<KeyValuePair<String, String>> Data => new Dictionary<String, String>
        {
            { "grant_type", GrantType },
            { "client_id", ID },
            { "client_secret", Secret },
            { "scope", Scope },
        };
    }
    
    partial class ClientCredential : ICredential
    {
        AuthenticationHeaderValue? ICredential.Authorization => default;

        HttpMethod? ICredential.Method => Method switch
        {
            String value => HttpMethod.Parse(value),
            _ => null,
        };
        Uri? ICredential.RequestUri => RequestUri;
        HttpContent ICredential.Content => new FormUrlEncodedContent(Data);
    }
}
