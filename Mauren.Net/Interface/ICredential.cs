using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Mauren.Net
{
    public interface ICredential
    {
        /// <summary>
        /// The <see cref="AuthenticationHeaderValue">Authorization header</see> to 
        /// when sending a request to the authentication endpoint.
        /// </summary>
        AuthenticationHeaderValue? Authorization { get; }

        /// <summary>
        /// The <see cref="HttpMethod">HTTP method</see> to use 
        /// when sending an authentication request.
        /// </summary>
        HttpMethod? Method { get; }

        /// <summary>
        /// The <see cref="Uri">endpoint</see> to use
        /// when sending an authentication request.
        /// </summary>
        Uri? RequestUri { get; }

        /// <summary>
        /// The <see cref="HttpContent"/> supplied as the body
        /// of the authorization grant request.
        /// </summary>
        HttpContent Content { get; }
    }

    public static class ICredentialExtensions
    {
        extension<TCredential>(TCredential) where TCredential : ICredential
        {
            /// <summary>
            /// A <see cref="String"/> matching the <see cref="IConfigurationSection"/>
            /// <see cref="IConfigurationSection.Key">key</see> that this
            /// <see cref="ICredential"/> instance should map to.
            /// </summary>
            public static String? Key
            {
                get
                {
                    Type type = typeof(TCredential);
                    try
                    {
                        return type.GetSectionName();
                    }
                    catch (CustomAttributeFormatException)
                    {
                        return type.FullName;
                    }
                }
            }
        }
    }
}