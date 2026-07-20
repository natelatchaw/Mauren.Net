using Mauren.Net;
using Mauren.Net.Attributes;
using Mauren.Net.Authentication;
using Mauren.Net.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using HttpRequestHeaders = Mauren.Net.Http.Headers.HttpRequestHeaders;


namespace Mauren.Net.Extensions
{
    public static partial class ApiServiceExtensions
    {
        public static IHttpRequestMessage CreateRequest<TCredential, TState, ContentType>(
            this JsonApiService<TCredential, TState> service,
            HttpMethod method,
            Uri requestUri,
            HttpRequestHeaders? requestHeaders = default,
            HttpRequestOptions? requestOptions = default,
            ContentType? content = default
        )
            where TCredential : class, ICredential
            where TState : IAuthenticationState, new()
        {
            JsonSerializerOptions? serializerOptions = service.SerializerOptions;
            return new Request<ContentType>(method, requestUri, requestHeaders, requestOptions, content, serializerOptions);
        }

        public static IHttpRequestMessage CreateRequest<TCredential, TState, ContentType>(
            this JsonApiService<TCredential, TState> service,
            HttpMethod method,
            IEndpoint endpoint,
            HttpRequestHeaders? requestHeaders = default,
            HttpRequestOptions? requestOptions = default,
            ContentType? content = default
        )
            where TCredential : class, ICredential
            where TState : IAuthenticationState, new()
        {
            Uri requestUri = service.GetRequestUri(endpoint)
                ?? throw new HttpRequestException($"The {service.GetType().Name} does not have a configured {nameof(HttpClient.BaseAddress)}.");
            return service.CreateRequest(method, requestUri, requestHeaders, requestOptions, content);
        }
        
        public static IHttpRequestMessage CreateRequest<TCredential, TState>(
            this JsonApiService<TCredential, TState> service,
            HttpMethod method,
            IEndpoint endpoint,
            HttpRequestHeaders? requestHeaders = default,
            HttpRequestOptions? requestOptions = default
        )
            where TCredential : class, ICredential
            where TState : IAuthenticationState, new()
        {
            Object? content = default;
            return service.CreateRequest(method, endpoint, requestHeaders, requestOptions, content);
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ApiServiceExtensions
    {
        [Obsolete("", error: true)]
        public static IServiceCollection AddApiService<TService, TAuthentication, TConfiguration, TEndpoint, TCredential, TState>(this IServiceCollection services, IConfiguration configuration, TimeSpan? timeout = default)
            where TService : ApiService<TCredential, TState>
            where TCredential : class, ICredential
            where TState : IAuthenticationState, new()
            where TAuthentication : class, Mauren.Net.Authentication.IAuthenticationRequestMessage, new()
            where TConfiguration : class, new()
        {
            /*
            // Determine the configuration section name for the configuration type
            String sectionName = typeof(TConfiguration).GetSectionName();
            // Get the configuration section via the section name
            IConfigurationSection section = configuration.GetSection(sectionName);
            */

            // Determine the full name of the service type
            if (typeof(TService).FullName is not String fullName)
                throw new Exception($"Could not determine the full name of {typeof(TService).Name}.");
            
            services
                // Configure the HttpClient service
                .AddHttpClient<TService>(fullName, (HttpClient client) =>
                {
                    if (timeout is TimeSpan value) client.Timeout = value;
                });

            services.AddOptions<TConfiguration>(configuration);
            services.AddOptions<TAuthentication>(configuration);

            /*
            services
                // Configure the configuration section
                .Configure<TConfiguration>(section);
            */

            // Return the service collection for chaining
            return services;
        }

        [Obsolete($"Use {nameof(OptionsBuilderExtensions.Bind)} instead", error: true)]
        public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class, new()
        {
            // Determine the configuration section name for the configuration type
            String sectionName = typeof(TOptions).GetSectionName();
            // Get the configuration section via the section name
            IConfigurationSection section = configuration.GetSection(sectionName);
            
            // Configure the configuration section
            services.Configure<TOptions>(section);

            // Return the service collection for chaining
            return services;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ApiServiceExtensions
    {
        public static IServiceCollection AddApiService<TApiService, TCredential, TState, TEndpoint>(this IServiceCollection services)
            where TApiService : ApiService<TCredential, TState>
            where TCredential : class, ICredential, new()
            where TState : IAuthenticationState, new()
            where TEndpoint : IEndpoint
        {
            String key = typeof(TApiService).GetKey();

            services
                .AddHttpClient<TApiService>(key)
                .AddHttpMessageHandler<AuthenticationHandler<TState>>();

            return services;
        }

        public static IServiceCollection AddApiService<TApiService, TCredential, TState>(this IServiceCollection services, IConfiguration configuration)
            where TApiService : ApiService<TCredential, TState>
            where TCredential : class, ICredential
            where TState : IAuthenticationState, new()
        {
            Type type = typeof(TApiService);
            ConfigurationSectionAttribute? attribute = type.GetCustomAttribute<ConfigurationSectionAttribute>();
            String? sectionName = attribute?.Name;
            if (String.IsNullOrWhiteSpace(sectionName))
            {
                throw new InvalidOperationException($"{type.FullName} is missing an {nameof(ConfigurationSectionAttribute)}");
            }

            IConfigurationSection section = configuration.GetRequiredSection(sectionName);
            if (section.Exists() is false)
            {
                throw new InvalidOperationException($"{type.FullName} expects an {nameof(IConfigurationSection)} with key '{sectionName}', but the section was missing in {nameof(IConfiguration)}");
            }
            Uri? baseAddress = section.GetValue<Uri>(nameof(HttpClient.BaseAddress));

            services.AddHttpClient<TApiService>(sectionName, (HttpClient httpClient) =>
            {
                httpClient.BaseAddress = baseAddress;
            });
            services.AddOptions<TCredential>().Bind(configuration as IConfigurationRoot);

            return services;
        }
    }
}

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <typeparam name="TConfiguration">
        /// A class with an applied <see cref="ConfigurationSectionNameAttribute"/>.
        /// </typeparam>
        /// 
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> instance to retrieve an <see cref="IConfigurationSection"/> from.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="IConfigurationSection"/>.
        /// </returns>
        /// 
        /// <exception cref="Exception"></exception>
        public static IConfigurationSection GetSection<TConfiguration>(this IConfiguration configuration)
        {
            // Get the section name of the TConfiguration type
            String sectionName = typeof(TConfiguration).GetSectionName();
            // Return the section with the section name
            return configuration.GetSection(sectionName);
        }
    }
}