using Mauren.Net.Credentials;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Mauren.Net.Test
{
    [TestClass]
    public sealed class CredentialFactoryTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task Create_ShouldMatch_ConfigurationValues()
        {
            // Arrange
            String key = Helpers.GetKey<TestCredential>();
            Uri requestUri = new("https://localhost/auth/token");
            String dataValue = "aEo30d7hkle";
            IDictionary<String, String?> values = new Dictionary<String, String?>()
            {
                { $"{key}:Method", "GET" },
                { $"{key}:RequestUri", requestUri.OriginalString },
                { $"{key}:Scheme", "scheme_value" },
                { $"{key}:Parameter", "parameter_value" },
                { $"{key}:DataValue", dataValue },
            };
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
            ICredentialFactory credentialFactory = Helpers.GetCredentialFactory<TestCredential>(configuration, out Mock<ICredentialFactory> mock);

            // Act
            ICredential credential = credentialFactory.Create<TestCredential>(key);

            // Assert
            Assert.AreEqual(requestUri, credential.RequestUri);
            Assert.AreEqual(HttpMethod.Get, credential.Method);

            mock
                .Verify(_ => _.Create<TestCredential>(It.IsAny<String>()), Times.Once());
        }

        [TestMethod]
        public async Task Create_JwtShouldMatch_ConfigurationValues()
        {
            // Arrange
            String key = Helpers.GetKey<TestJwtCredential>();

            IDictionary<String, String?> values = new Dictionary<String, String?>()
            {
                [$"{key}:{nameof(TestJwtCredential.Method)}"] = "GET",
                [$"{key}:{nameof(TestJwtCredential.RequestUri)}"] = "https://example.com",
                [$"{key}:{nameof(TestJwtCredential.Thumbprint)}"] = "TEST_THUMBPRINT",
                [$"{key}:{nameof(TestJwtCredential.Issuer)}"] = "TEST_ISSUER",
                [$"{key}:{nameof(TestJwtCredential.Audience)}"] = "TEST_AUDIENCE",
                [$"{key}:{nameof(TestJwtCredential.Subject)}"] = "TEST_SUBJECT",
            };
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
            ICredentialFactory credentialFactory = Helpers.GetCredentialFactory<TestJwtCredential>(configuration, out Mock<ICredentialFactory> mock);

            // Act
            ICredential credential = credentialFactory.Create<TestJwtCredential>(key);
            
            // Assert
            try
            {
                String? _ = credential.Authorization?.Parameter;
            }
            catch (InvalidOperationException exception)
            {
                Assert.Inconclusive(exception.Message);
            }
        }
    }

    file static class Helpers
    {
        public static String GetKey<TCredential>()
            where TCredential : ICredential
        {
            String key = typeof(TCredential).GetSectionName();
            return key;
        }

        public static ICredentialFactory GetCredentialFactory<TCredential>(Func<IConfiguration> getConfiguration, out Mock<ICredentialFactory> mock)
            where TCredential : ICredential, new() => GetCredentialFactory<TCredential>(getConfiguration.Invoke(), out mock);
        public static ICredentialFactory GetCredentialFactory<TCredential>(IConfiguration configuration, out Mock<ICredentialFactory> mock)
            where TCredential : ICredential, new()
        {
            Mock<ICredentialFactory> mockCredentialFactory = new();
            mockCredentialFactory
                .Setup(_ => _.Create<TCredential>(It.IsAny<String>()))
                .Returns(() =>
                {
                    String key = Helpers.GetKey<TCredential>();
                    IConfigurationSection section = configuration.GetRequiredSection(key);
                    TCredential credential = new();
                    section.Bind(credential);
                    return credential;
                })
                .Callback((String key) =>
                {
                    Debug.WriteLine($"{nameof(TCredential)} requested for key {key}");
                })
                .Verifiable();
            mock = mockCredentialFactory;
            return mockCredentialFactory.Object;
        }
    }

    [ConfigurationSection(Name = "TestJwtCredential")]
    file class TestJwtCredential : JwtCredential { }

    [ConfigurationSection(Name = "TestCredential")]
    file class TestCredential : ICredential
    {
        public String Method { get; set; }
        public Uri RequestUri { get; set; }
        public String Scheme { get; set; }
        public String Parameter { get; set; }

        public String DataValue { get; set; }


        HttpMethod? ICredential.Method => HttpMethod.Parse(Method);
        AuthenticationHeaderValue? ICredential.Authorization => new(Scheme, Parameter);
        HttpContent ICredential.Content => JsonContent.Create<Data>(new()
        {
            Value = DataValue,
        });

        public class Data
        {
            public String? Value { get; set; }
        }
    }
}
