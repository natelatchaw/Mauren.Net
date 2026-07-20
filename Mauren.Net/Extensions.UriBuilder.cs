using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Mauren.Net
{
    public static class UriBuilderExtensions
    {
        extension(UriBuilder uriBuilder)
        {
            /// <summary>
            /// A helper <see langword="extension"/> property that applies the provided
            /// <see cref="NameValueCollection">query parameters</see> to the <see cref="UriBuilder.Query"/> property.
            /// </summary>
            public NameValueCollection QueryParameters
            {
                set
                {
                    // Assign the provided value
                    NameValueCollection queryParameters = value;
                    // Get an empty NameValueCollection to collect query parameters in
                    NameValueCollection collected = HttpUtility.ParseQueryString(String.Empty);
                    // Get a dictionary of query parameters provided by the endpoint
                    IDictionary<String, String[]?> parameters = queryParameters.AllKeys
                        // Filter out null keys and apply Null-Forgiving operator to each key
                        .Where((String? key) => key != null).Select((String? key) => key!)
                        // Convert to key value pairs to a dictionary
                        .ToDictionary((String key) => key, (String key) => queryParameters.GetValues(key), StringComparer.OrdinalIgnoreCase);

                    // For each key value pair in the query parameters provided by the endpoint
                    foreach (KeyValuePair<String, String[]?> parameter in parameters)
                    {
                        // If no values were provided by the query parameter, skip the parameter
                        if (parameter.Value == null) continue;
                        // Add the key value pair to the collection of query parameters
                        collected.Add(parameter.Key, String.Join(',', parameter.Value));
                    }

                    // Assign the assembled query parameter string to the builder's Query property
                    uriBuilder.Query = collected.ToString();
                }
            }

            /// <summary>
            /// A helper <see langword="extension"/> property that applies the provided
            /// <see cref="IList{T}">path segments</see> to the <see cref="UriBuilder.Path"/> property.
            /// </summary>
            public IList<String> PathSegments
            {
                set
                {
                    // Assign the provided value
                    IEnumerable<String> pathSegments = value
                        // Trim any leading or trailing forward slashes from path segments
                        .Select((String pathSegment) => pathSegment.Trim('/'));

                    // Join path segments with a forward slash seperator
                    String path = String.Join('/', pathSegments);

                    // Assign the assembled path string to the builder's Path property
                    uriBuilder.Path = path;
                }
            }
        }
    }
}
