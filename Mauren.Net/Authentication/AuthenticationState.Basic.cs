using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Mauren.Net.Authentication
{
    /// <summary>
    /// Tracks the state of basic token based authentication.
    /// </summary>
    public class BasicAuthenticationState : AuthenticationState
    {
        public BasicAuthenticationState() : base() { }

        #region overrides

        protected override String Scheme { get; } = "Basic";

        /// <remarks>
        /// See <see href="https://datatracker.ietf.org/doc/html/rfc7617#section-2">RFC 7617 Section 2</see>
        /// for details.
        /// </remarks>
        /// <inheritdoc/>
        protected override String? Parameter
        {
            get
            {
                if (Credential is not ResourceOwnerPasswordCredential credential)
                    throw new InvalidOperationException($"Invalid credential provided");
                // Construct the user-pass by concatenating the user-id, a single colon character, and the password
                String userpass = $"{credential.Username}:{credential.Password}";
                // Encodes the user-pass into an octet sequence
                Byte[] bytes = Encoding.GetBytes(userpass);
                // Obtain the basic credentials by encoding this octet sequence using Base64 into a sequence of US-ASCII characters
                String parameter = System.Convert.ToBase64String(bytes);
                return parameter;
            }
        }

        /// <remarks>
        /// Returns <see langword="default"/> unconditionally, as the Basic Authentication scheme
        /// does not require a <see cref="HttpRequestMessage"/> to obtain credentials.
        /// </remarks>
        /// <inheritdoc/>
        public override HttpRequestMessage? Request
        {
            get => default;
        }

        /// <remarks>
        /// Throws <see cref="NotImplementedException"/> unconditionally, as the Basic Authentication scheme
        /// does not require a <see cref="HttpResponseMessage"/> to obtain credentials.
        /// </remarks>
        /// <inheritdoc/>
        public override HttpResponseMessage? Response
        {
            set => throw new NotImplementedException();
        }

        /// <remarks>
        /// Returns <see langword="false"/> unconditionally, as the Basic Authentication scheme
        /// does not benefit from being retried.
        /// </remarks>
        /// <inheritdoc/>
        public override Boolean TryReset(HttpStatusCode? statusCode = null)
        {
            return false;
        }

        #endregion


        #region implementation

        /// <summary>
        /// The request containing credentials to use for authorization.
        /// </summary>
        //public AuthenticationRequestMessage<ResourceOwnerPasswordCredential> RequestMessage { get; set; }

        /// <summary>
        /// The character set to use when encoding credentials.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Encoding.ASCII"/>
        /// </remarks>
        public Encoding Encoding { get; set; } = Encoding.ASCII;

        #endregion
    }
}
