using Mauren.Net.Attributes;
using Mauren.Net.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mauren.Net.Test
{
    [ApiService(Name = "MockService")]
    internal class MockService : ApiService<MockCredential, MockAuthenticationState>
    {
        public MockService(HttpClient httpClient, MockCredential credential) : base(httpClient, credential)
        {
        }

        /// <summary>
        /// Retrieves a reference to the base class' <see langword="protected"/>
        /// <see cref="ApiService{TEndpoint, TCredential, TState}.State">state</see>
        /// value for testing purposes.
        /// </summary>
        internal new MockAuthenticationState State { get => base.State; }

        /// <summary>
        /// Retrieves the <see langword="abstract"/> <see langword="base"/> <see langword="class"/>'s
        /// <c>Authorization</c> header parameter for testing purposes.
        /// </summary>
        /// 
        /// <remarks>
        /// As this <see langword="class"/> implements <see cref="BearerAuthenticationState{TAuthenticationResponseContent}"/>,
        /// this value represents an access token.
        /// </remarks>
        internal String? AccessToken { get => base.State.Parameter; }

        /// <summary>
        /// Calls the <see langword="abstract"/> <see langword="base"/> <see langword="class"/>'s
        /// <see cref="AuthenticateAsync"/> method for testing purposes.
        /// </summary>
        /// <returns></returns>
        internal new Task AuthenticateAsync() => base.HttpClient.UpdateAsync(State);
    }

    internal class MockAuthenticationState : BearerAuthenticationState<MockResponseContent>
    {
        /// <summary>
        /// Retreives a reference to the base class' <see langword="protected"/>
        /// <see cref="BearerAuthenticationState{TAuthenticationResponseContent}.Parameter">parameter</see>
        /// value for testing purposes.
        /// </summary>
        internal new String? Parameter { get => base.Parameter; }
    }
}
