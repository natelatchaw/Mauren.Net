using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Overrides the default <see cref="HttpMessageHandler"/> for <typeparamref name="TClient"/>
        /// with the provided <paramref name="httpMessageHandler"/>.
        /// </summary>
        /// 
        /// <typeparam name="TClient">
        /// A service with a <see cref="HttpClient"/> injected via constructor.
        /// </typeparam>
        /// 
        /// <param name="services"></param>
        /// 
        /// <param name="httpMessageHandler">
        /// The <see cref="HttpMessageHandler"/> to override the existing implementation with.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="IServiceCollection"/> for chaining.
        /// </returns>
        public static IServiceCollection OverrideHttpMessageHandler<TClient>(this IServiceCollection services, HttpMessageHandler httpMessageHandler)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(httpMessageHandler, nameof(httpMessageHandler));

            // Register handler
            services.AddTransient<HttpMessageHandlerWrapper>(_ => new(typeof(TClient), httpMessageHandler));
            // Create a service descriptor for HttpMessageHandlerBuilderFilter
            ServiceDescriptor descriptor = ServiceDescriptor.Transient<IHttpMessageHandlerBuilderFilter, HttpMessageHandlerBuilderFilter>();
            // Replace the existing IHttpMessageHandlerBuilderFilter implementation with the descriptor
            services.Replace(descriptor);

            return services;
        }
    }

    public sealed class HttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IEnumerable<HttpMessageHandlerWrapper> _wrappers;

        public HttpMessageHandlerBuilderFilter(IEnumerable<HttpMessageHandlerWrapper> wrappers)
        {
            _wrappers = wrappers;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) => (HttpMessageHandlerBuilder builder) =>
        {
            HttpMessageHandlerWrapper? wrapper = _wrappers
                .Where((HttpMessageHandlerWrapper wrapper) => wrapper.HttpClientType.Name.Equals(builder.Name, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
            
            if (wrapper is HttpMessageHandlerWrapper selected)
            {
                Debug.WriteLine($"Overriding {nameof(builder.PrimaryHandler)} for {builder.Name} HTTP client");
                builder.PrimaryHandler = selected.HttpMessageHandler;
            }
            
            next(builder);
        };
    }

    public sealed class HttpMessageHandlerWrapper
    {
        public Type HttpClientType { get; set; }
        public HttpMessageHandler HttpMessageHandler { get; set; }

        public HttpMessageHandlerWrapper(Type httpClientType, HttpMessageHandler httpMessageHandler)
        {
            HttpClientType = httpClientType;
            HttpMessageHandler = httpMessageHandler;
        }
    }
}
