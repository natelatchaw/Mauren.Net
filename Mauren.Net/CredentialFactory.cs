using Mauren.Net;
using Mauren.Net.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net
{
    public interface ICredentialFactory
    {
        /// <summary>
        /// Create a <typeparamref name="TCredential"/> registered under the provided <paramref name="key"/>.
        /// </summary>
        /// 
        /// <typeparam name="TCredential">
        /// The type of credential to create.
        /// </typeparam>
        /// 
        /// <param name="key">
        /// The key that the credential is registered under.
        /// </param>
        /// 
        /// <returns>
        /// The created <typeparamref name="TCredential"/>.
        /// </returns>
        public TCredential Create<TCredential>(String key) where TCredential : ICredential, new();
    }

    internal class CredentialFactory : ICredentialFactory
    {
        private readonly IConfiguration _configuration;

        public CredentialFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        TCredential ICredentialFactory.Create<TCredential>(String key)
        {
            // Retrieve the configuration section by the provided key
            IConfigurationSection section = _configuration.GetRequiredSection(key);
            // Create an empty credential
            TCredential credential = new();
            // Bind the section to the created credential
            section.Bind(credential);
            // Return the credential
            return credential;
        }
    }

    public class AuthenticationHandler<TAuthenticationState> : DelegatingHandler
        where TAuthenticationState : IAuthenticationState, new()
    {
        private TAuthenticationState _state;

        public AuthenticationHandler(IAuthenticationStateFactory factory)
        {
            this._state = factory.Create<TAuthenticationState>(String.Empty);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue? authorization = _state.Header;
            request.Headers.Authorization = authorization;
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Boolean shouldReset = (response.StatusCode) switch
            {
                System.Net.HttpStatusCode.Unauthorized => true,
                _ => false,
            };
            if (shouldReset)
            {
                //_state.TryReset(response.StatusCode);
            }

            return response;
        }

    }
}

namespace Microsoft.Extensions.Configuration
{
    public static class CredentialFactoryExtensions
    {
        public static ICredentialFactory GetCredentialFactory(this IConfiguration configuration)
        {
            return new CredentialFactory(configuration);
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CredentialFactoryExtensions
    {
        public static IServiceCollection AddCredentialFactory(this IServiceCollection services) => services
            .AddTransient<ICredentialFactory, CredentialFactory>((IServiceProvider provider) =>
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                return new CredentialFactory(configuration);
            });
    }
}