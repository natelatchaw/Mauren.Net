using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Mauren.Net
{
    public static class IEndpointExtensions
    {
        /// <summary>
        /// Applies the provided <paramref name="endpoint"/> to the <paramref name="baseAddress"/>
        /// and returns the resulting <see cref="Uri"/>.
        /// </summary>
        /// 
        /// <param name="baseAddress">
        /// A <see cref="Uri"/> representing the base address.
        /// </param>
        /// 
        /// <param name="endpoint">
        /// An <see cref="IEndpoint"/> containing endpoint information (path segments, query parameters, etc.)
        /// </param>
        /// 
        /// <returns>
        /// An assembled <see cref="Uri"/> containing the provided information.
        /// </returns>
        [return: NotNullIfNotNull(nameof(baseAddress))]
        public static Uri? At(this Uri? baseAddress, IEndpoint endpoint)
        {
            // If the provided base address is null, return null
            if (baseAddress is null) return null;

            // Create a new UriBuilder from the provided base Uri address
            UriBuilder uriBuilder = new(baseAddress)
            {
                // Apply the endpoint's query parameters
                QueryParameters = endpoint.QueryParameters,

                // Apply the endpoint's path segments
                PathSegments = endpoint.PathSegments,
            };

            // Return the assembled Uri
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Applies the provided <paramref name="endpoint"/> to the <paramref name="httpClient"/>'s
        /// <see cref="HttpClient.BaseAddress"/> and returns the resulting <see cref="Uri"/>.
        /// </summary>
        /// 
        /// <param name="httpClient">
        /// An <see cref="HttpClient"/> with a defined <see cref="HttpClient.BaseAddress"/>.
        /// </param>
        /// 
        /// <param name="endpoint">
        /// An <see cref="IEndpoint"/> containing endpoint information (path segments, query parameters, etc.)
        /// </param>
        /// 
        /// <returns>
        /// An assembled <see cref="Uri"/> containing the provided information.
        /// </returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Thrown if the provided <paramref name="httpClient"/> does not have 
        /// an assigned <see cref="HttpClient.BaseAddress"/>.
        /// </exception>
        public static Uri At(this HttpClient httpClient, IEndpoint endpoint)
        {
            if (httpClient.BaseAddress is not Uri baseAddress)
                throw new InvalidOperationException($"The provided {nameof(HttpClient)} does not have an assigned {nameof(HttpClient.BaseAddress)}");
            return baseAddress.At(endpoint);
        }

        [return: NotNullIfNotNull(nameof(baseAddress))]
        public static Uri? ToUri(this IEndpoint endpoint, Uri? baseAddress)
        {
            try
            {
                // If the base address is null, throw exception
                ArgumentNullException.ThrowIfNull(baseAddress);

                // Create a new UriBuilder from the provided base Uri address
                UriBuilder uriBuilder = new(baseAddress)
                {
                    // Apply the endpoint's query parameters
                    QueryParameters = endpoint.QueryParameters,

                    // Apply the endpoint's path segments
                    PathSegments = endpoint.PathSegments,
                };

                // Return the assembled Uri
                return uriBuilder.Uri;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
    }
}