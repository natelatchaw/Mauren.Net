using Mauren.Net.Authentication;
using System;
using System.Text.Json.Serialization;

namespace Mauren.Net.Test
{
    internal class MockResponseContent : IBearerAuthenticationResponseContent
    {
        [JsonPropertyName("accessToken")]
        public required String AccessToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public required String RefreshToken { get; set; }
        [JsonPropertyName("expiresIn")]
        public required Int32 ExpiresIn { get; set; }
    }
}
