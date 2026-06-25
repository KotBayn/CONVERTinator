using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using CONVERTinator.Services.Providers;
using CONVERTinator.Domain;

namespace CONVERTinator.Tests.Providers
{
    public class RegionalFloatRatesProviderTests
    {
        [Fact]
        public async Task GetRatesAsync_WithValidJson_ParsesCorrectly()
        {
            // Arrange: fake JSON-answer (imitate floatrates)
            string fakeJson = @"{
                ""usd"": { ""code"": ""USD"", ""name"": ""US Dollar"", ""rate"": 1.05 },
                ""jpy"": { ""code"": ""JPY"", ""name"": ""Japanese Yen"", ""rate"": ""150.5"" }
            }";

            // Moq
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeJson) 
                });

            var fakeHttpClient = new HttpClient(handlerMock.Object);
            var provider = new RegionalFloatRatesProvider(fakeHttpClient, "eur", "TestBank");

            // Act
            var result = await provider.GetRatesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(c => c.Code == "USD" && c.Value == 1.05m);
            result.Should().ContainSingle(c => c.Code == "JPY" && c.Value == 150.5m);
        }
    }
}