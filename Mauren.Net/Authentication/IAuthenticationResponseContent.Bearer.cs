namespace Mauren.Net.Authentication
{
    /// <summary>
    /// Represents the response content received from an authentication request
    /// with the <c>Bearer</c> scheme.
    /// </summary>
    /// 
    /// <remarks>
    /// <inheritdoc/>
    /// </remarks>
    public interface IBearerAuthenticationResponseContent : IAuthenticationResponseContent
    {
        /// <summary>
        /// The field representing the provided <c>Access</c> token.
        /// </summary>
        System.String AccessToken { get; set; }

        /// <summary>
        /// The field representing the provided <c>Refresh</c> token.
        /// </summary>
        System.String RefreshToken { get; set; }

        /// <summary>
        /// The field representing the provided expiration value in seconds.
        /// </summary>
        System.Int32 ExpiresIn { get; set; }
    }
}
