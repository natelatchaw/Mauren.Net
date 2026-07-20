using Mauren.Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Mauren.Net
{
    public interface IEndpoint
    {
        /// <summary>
        /// A list of path segments to be applied to the <see cref="IEndpoint"/>.
        /// </summary>
        List<String> PathSegments { get; set; }

        /// <summary>
        /// A collection of query parameters to be applied to the <see cref="IEndpoint"/>.
        /// </summary>
        NameValueCollection QueryParameters { get; set; }
    }
}

namespace FluentEndpointBuilder
{
    public static class EndpointExtensions
    {
        public static EndpointType PageNumber<EndpointType>(this EndpointType endpoint, Int32 number, String key = "PageNumber")
            where EndpointType : IEndpoint
        {
            endpoint.QueryParameters.Set(key, number.ToString());
            return endpoint;
        }

        public static EndpointType PageSize<EndpointType>(this EndpointType endpoint, Int32 size, String key = "PageSize")
            where EndpointType : IEndpoint
        {
            endpoint.QueryParameters.Set(key, size.ToString());
            return endpoint;
        }
    }
}