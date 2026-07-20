using System;
using System.Net;
using System.Net.Http;

namespace Mauren.Net.Authentication
{
    public class EmptyAuthenticationState : AuthenticationState
    {
        public override HttpRequestMessage? Request => default;

        public override HttpResponseMessage? Response { set => _ = value; }

        protected override String Scheme => String.Empty;

        protected override String? Parameter => default;

        public override Boolean TryReset(HttpStatusCode? statusCode = null)
        {
            return false;
        }
    }
}
