using Moq;
using Moq.Protected;
using System;
using System.Net.Http;

namespace Mauren.Net.Test
{
    static partial class Helpers
    {
        public static HttpMessageHandler GetHttpMessageHandler(Action<IProtectedAsMock<HttpMessageHandler, IHttpMessageHandler>> configure)
        {
            // Create the mocked HttpMessageHandler
            Mock<HttpMessageHandler> mockHttpMessageHandler = new();
            // Utilize interface for mocking
            IProtectedAsMock<HttpMessageHandler, IHttpMessageHandler> value = mockHttpMessageHandler
                // Mock protected interface
                .Protected().As<IHttpMessageHandler>();

            // Invoke the configuration action
            configure.Invoke(value);

            // Return the mocked object
            return mockHttpMessageHandler.Object;
        }
    }
}
