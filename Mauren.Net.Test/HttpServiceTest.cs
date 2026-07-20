using Mauren.Net.Authentication;
using Mauren.Net.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net.Test
{
    [TestClass]
    public sealed class ApiServiceTest
    {
        public TestContext TestContext { get; set; }

        const String ACCESS_TOKEN = "EXPECTED_ACCESS_TOKEN_VALUE";
        const String REFRESH_TOKEN = "EXPECTED_REFRESH_TOKEN_VALUE";

        [TestMethod]
        public async Task AuthenticateAsync_ShouldMatch_ReceivedAccessToken()
        {
            // Arrange
            HttpResponseMessage responseMessage = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create<MockResponseContent>(new()
                {
                    AccessToken = ACCESS_TOKEN,
                    RefreshToken = REFRESH_TOKEN,
                    ExpiresIn = 5000,
                }, mediaType: new MediaTypeHeaderValue("application/json"))
            };
            HttpMessageHandler httpMessageHandler = Helpers.GetHttpMessageHandler(responseMessage, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            IHttpClientFactory httpClientFactory = Helpers.GetHttpClientFactory(httpMessageHandler, out Mock<IHttpClientFactory> _);
            ICredentialFactory credentialFactory = Helpers.GetCredentialFactory(() =>
            {
                return new MockCredential
                {
                    Authorization = null
                };
            }, out Mock<ICredentialFactory> mockCredentialFactory);
            IAuthenticationStateFactory authenticationStateFactory = Helpers.GetAuthenticationStateFactory(() =>
            {
                return new MockAuthenticationState
                {

                };
            }, out Mock<IAuthenticationStateFactory> _);

            HttpClient httpClient = httpClientFactory.CreateClient(String.Empty);
            MockCredential credential = credentialFactory.Create<MockCredential>(String.Empty);
            MockAuthenticationState state = authenticationStateFactory.Create<MockAuthenticationState>(String.Empty);

            MockService service = new(httpClient, credential);


            //Act
            await service.AuthenticateAsync();

            // Assert
            Assert.AreEqual(ACCESS_TOKEN, service.AccessToken, "Received access token does not match the expected access token.");
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldThrow_IfReceivedAccessTokenIsExpired()
        {
            // Arrange
            HttpResponseMessage responseMessage = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create<MockResponseContent>(new()
                {
                    AccessToken = ACCESS_TOKEN,
                    RefreshToken = REFRESH_TOKEN,
                    ExpiresIn = -60,
                }, mediaType: new MediaTypeHeaderValue("application/json"))
            };
            HttpMessageHandler httpMessageHandler = Helpers.GetHttpMessageHandler(responseMessage, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            IHttpClientFactory httpClientFactory = Helpers.GetHttpClientFactory(httpMessageHandler, out Mock<IHttpClientFactory> _);
            ICredentialFactory credentialFactory = Helpers.GetCredentialFactory(() =>
            {
                return new MockCredential
                {
                    Authorization = null
                };
            }, out Mock<ICredentialFactory> mockCredentialFactory);
            IAuthenticationStateFactory authenticationStateFactory = Helpers.GetAuthenticationStateFactory(() =>
            {
                return new MockAuthenticationState
                {

                };
            }, out Mock<IAuthenticationStateFactory> _);

            HttpClient httpClient = httpClientFactory.CreateClient(String.Empty);
            MockCredential credential = credentialFactory.Create<MockCredential>(String.Empty);
            MockAuthenticationState state = authenticationStateFactory.Create<MockAuthenticationState>(String.Empty);

            MockService service = new(httpClient, credential);


            // Act
            await service.AuthenticateAsync();

            Assert.Throws<InvalidAuthenticationValueException>(() => service.State.Parameter);
            Assert.IsNull(service.State.Header);
        }

        [TestMethod]
        public async Task SendAsync_ShouldReturn_ExpectedAuthorizationHeader()
        {
            // Arrange
            HttpResponseMessage responseMessage = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create<MockResponseContent>(new()
                {
                    AccessToken = ACCESS_TOKEN,
                    RefreshToken = REFRESH_TOKEN,
                    ExpiresIn = 5000,
                }, mediaType: new MediaTypeHeaderValue("application/json"))
            };
            HttpMessageHandler httpMessageHandler = Helpers.GetHttpMessageHandler(responseMessage, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            IHttpClientFactory httpClientFactory = Helpers.GetHttpClientFactory(httpMessageHandler, out Mock<IHttpClientFactory> _);
            ICredentialFactory credentialFactory = Helpers.GetCredentialFactory(() =>
            {
                return new MockCredential
                {
                    Authorization = null
                };
            }, out Mock<ICredentialFactory> mockCredentialFactory);
            IAuthenticationStateFactory authenticationStateFactory = Helpers.GetAuthenticationStateFactory(() =>
            {
                return new MockAuthenticationState
                {

                };
            }, out Mock<IAuthenticationStateFactory> _);

            HttpClient httpClient = httpClientFactory.CreateClient(String.Empty);
            MockCredential credential = credentialFactory.Create<MockCredential>(String.Empty);
            MockAuthenticationState state = authenticationStateFactory.Create<MockAuthenticationState>(String.Empty);

            MockService service = new(httpClient, credential);

            // Act
            HttpResponseMessage response = await service.SendAsync(HttpMethod.Get, new("https://localhost/api/example"));

            // Assert
            mockHttpMessageHandler
                .Protected().As<IHttpMessageHandler>()
                .Verify(
                    _ => _.SendAsync(
                        It.Is((HttpRequestMessage request) => 
                            request.Headers.Authorization == null
                        ),
                        It.IsAny<CancellationToken>()
                    ),
                    Times.Exactly(1)
                );

            mockHttpMessageHandler
                .Protected().As<IHttpMessageHandler>()
                .Verify(
                    _ => _.SendAsync(
                        It.Is((HttpRequestMessage request) =>
                            request.Headers.Authorization != null &&
                            request.Headers.Authorization.Parameter == ACCESS_TOKEN
                        ),
                        It.IsAny<CancellationToken>()
                    ),
                    Times.Exactly(1)
                );
        }
    }
    
    file static class Helpers
    {
        public static HttpMessageHandler GetHttpMessageHandler(Func<HttpResponseMessage> getHttpResponseMessage, out Mock<HttpMessageHandler> mock) =>
            GetHttpMessageHandler(getHttpResponseMessage.Invoke(), out mock);
        public static HttpMessageHandler GetHttpMessageHandler(HttpResponseMessage httpResponseMessage, out Mock<HttpMessageHandler> mock)
        {
            Mock<HttpMessageHandler> mockHttpMessageHandler = new(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected().As<IHttpMessageHandler>()
                .Setup(_ => _.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponseMessage)
                .Callback((HttpRequestMessage requestMessage, CancellationToken cancellationToken) =>
                {
                    Debug.WriteLine(requestMessage.ToDebugString());
                })
                .Verifiable();
            mockHttpMessageHandler
                .Protected().As<IHttpMessageHandler>()
                .Setup(_ => _.Dispose(It.IsAny<Boolean>()))
                .Callback((Boolean? disposing) =>
                {
                    Debug.WriteLine($"Disposing {nameof(HttpMessageHandler)}");
                })
                .Verifiable();
            mock = mockHttpMessageHandler;
            return mockHttpMessageHandler.Object;
        }

        public static IHttpClientFactory GetHttpClientFactory(Func<HttpMessageHandler> getHttpMessageHandler, out Mock<IHttpClientFactory> mock) =>
            GetHttpClientFactory(getHttpMessageHandler.Invoke(), out mock);
        public static IHttpClientFactory GetHttpClientFactory(HttpMessageHandler httpMessageHandler, out Mock<IHttpClientFactory> mock)
        {
            Mock<IHttpClientFactory> mockHttpClientFactory = new();
            mockHttpClientFactory
                .Setup(_ => _.CreateClient(It.IsAny<String>()))
                .Returns(() => new HttpClient(httpMessageHandler));
            mock = mockHttpClientFactory;
            return mockHttpClientFactory.Object;
        }

        public static ICredentialFactory GetCredentialFactory<TCredential>(Func<TCredential> getCredential, out Mock<ICredentialFactory> mock)
            where TCredential : ICredential, new() => GetCredentialFactory(getCredential.Invoke(), out mock);
        public static ICredentialFactory GetCredentialFactory<TCredential>(TCredential credential, out Mock<ICredentialFactory> mock)
            where TCredential : ICredential, new()
        {
            Mock<ICredentialFactory> mockCredentialFactory = new();
            mockCredentialFactory
                .Setup(_ => _.Create<TCredential>(It.IsAny<String>()))
                .Returns(() => credential)
                .Callback((String key) =>
                {
                    Debug.WriteLine($"{nameof(TCredential)} requested for key {key}");
                })
                .Verifiable();
            mock = mockCredentialFactory;
            return mockCredentialFactory.Object;
        }

        public static IAuthenticationStateFactory GetAuthenticationStateFactory<TAuthenticationState>(Func<TAuthenticationState> getState, out Mock<IAuthenticationStateFactory> mock)
            where TAuthenticationState : IAuthenticationState, new() => GetAuthenticationStateFactory(getState.Invoke(), out mock);
        public static IAuthenticationStateFactory GetAuthenticationStateFactory<TAuthenticationState>(TAuthenticationState authenticationState, out Mock<IAuthenticationStateFactory> mock)
            where TAuthenticationState : IAuthenticationState, new()
        {
            Mock<IAuthenticationStateFactory> mockAuthenticationStateFactory = new();
            mockAuthenticationStateFactory
                .Setup(_ => _.Create<TAuthenticationState>(It.IsAny<String>()))
                .Returns(() => authenticationState);
            mock = mockAuthenticationStateFactory;
            return mockAuthenticationStateFactory.Object;
        }
    }
}
