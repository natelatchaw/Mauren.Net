using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Mauren.Net.Credentials
{
    public partial class JwtCredential
    {
        public String? Method { get; set; }
        public Uri? RequestUri { get; set; }
        public String? Thumbprint { get; set; }
        public String? Algorithm { get; set; } = SecurityAlgorithms.RsaSha256;

        /// <summary>
        /// Gets or sets the issuer of the <see cref="SecurityTokenDescriptor"/>.
        /// </summary>
        public String? Issuer { get; set; }

        /// <summary>
        /// Gets or sets the value of the {"": audience} claim. Will be combined with <see cref="Audiences"/> and any "Aud" claims in
        /// <see cref="Claims"/> or <see cref="Subject"/> when creating a token.
        /// </summary>
        public String? Audience { get; set; }

        public String? Subject { get; set; }
    }

    partial class JwtCredential : ICredential
    {
        X509Certificate2 Certificate
        {
            get
            {
                using X509Store store = new(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                if (Thumbprint is null)
                    throw new InvalidOperationException($"No certificate found in {store.Location}: No thumbprint provided");
                X509Certificate2? certificate = store.Certificates
                    .Find(X509FindType.FindByThumbprint, Thumbprint, validOnly: false)
                    .SingleOrDefault();
                if (certificate is null)
                    throw new InvalidOperationException($"No certificate found in {store.Location}: No thumbprint matching {Thumbprint}");
                return certificate;
            }
        }

        [SupportedOSPlatform("windows")]
        SecurityKey Key 
        {
            get
            {
                RSA publicKey = Certificate.GetRSAPublicKey() switch
                {
                    RSA value => value,
                    _ => throw new InvalidOperationException($"Certificate {Certificate.FriendlyName} does not have a {typeof(RSA)} public key."),
                };
                RSA? privateKey = Certificate.GetRSAPrivateKey() switch
                {
                    RSA value => value,
                    _ => throw new InvalidOperationException($"Certificate {Certificate.FriendlyName} does not have a {typeof(RSA)} private key."),
                };
                return new RsaSecurityKey(privateKey);
            }
        }

        [SupportedOSPlatform("windows")]
        SigningCredentials Credentials => new(Key, Algorithm);

        [SupportedOSPlatform("windows")]
        SecurityTokenDescriptor TokenDescriptor
        {
            get
            {
                Dictionary<String, Object> claims = new()
                {
                    //[ClaimTypes.Name] = Name,
                    //[ClaimTypes.System] = System,
                    [JwtRegisteredClaimNames.Jti] = Guid.NewGuid(),
                };
                IEnumerable<Claim> subject = new Claim[]
                {
                    new(JwtRegisteredClaimNames.Sub, Subject),
                };
                SecurityTokenDescriptor descriptor = new()
                {
                    SigningCredentials = Credentials,
                    Issuer = Issuer,
                    Audience = Audience,
                    Subject = new(subject),
                    Claims = claims,
                    TokenType = "JWT",
                    IssuedAt = DateTime.UtcNow,
                    NotBefore = DateTime.UtcNow.AddMinutes(-1),
                    Expires = DateTime.UtcNow.AddMinutes(120),
                };
                return descriptor;
            }
        }

        [SupportedOSPlatform("windows")]
        String Parameter
        {
            get
            {
                JsonWebTokenHandler handler = new()
                {
                    SetDefaultTimesOnTokenCreation = false
                };
                return handler.CreateToken(TokenDescriptor);
            }
        }

        HttpMethod? ICredential.Method => HttpMethod.Parse(Method);

        Uri? ICredential.RequestUri => RequestUri;

        [SupportedOSPlatform("windows")]
        AuthenticationHeaderValue? ICredential.Authorization => new("Bearer", Parameter);

        HttpContent ICredential.Content => throw new NotImplementedException();
    }
}
