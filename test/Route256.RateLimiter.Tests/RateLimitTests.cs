using Microsoft.AspNetCore.Mvc.Testing;
using Route256.RateLimiter.Demo;

namespace Route256.RateLimiter.Tests;

public class Tests
{
    private const string apiRateLimitPath = "/products";

    private WebApplicationFactory<Startup> _factory = new();
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7085")
        });
    }

    [Test]
    public async Task GlobalIpRule_UnderLimit()
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 5; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiRateLimitPath);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GlobalIpRule_ExceedLimit()
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 6; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiRateLimitPath);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(429));
    }
    
    [Test]
    [TestCase("84.247.85.224")]
    public async Task IndividualIpRule_UnderLimit(string ipAddress)
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 2; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiRateLimitPath);
            request.Headers.Add("IP_address", ipAddress);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(200));
    }
    
    [Test]
    [TestCase("84.247.85.225")]
    public async Task IndividualIpRule_ExceedLimit(string ipAddress)
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 3; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiRateLimitPath);
            request.Headers.Add("IP_address", ipAddress);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(429));
    }
    
    [Test]
    [TestCase("84.247.85.224")]
    public async Task ControllerRule_UnderLimit(string ipAddress)
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 1; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiRateLimitPath}/1");
            request.Headers.Add("IP_address", ipAddress);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(200));
    }
    
    [Test]
    [TestCase("84.247.85.225")]
    public async Task ControllerRule_ExceedLimit(string ipAddress)
    {
        int responseStatusCode = 0;

        for (int i = 0; i < 2; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiRateLimitPath}/1");
            request.Headers.Add("IP_address", ipAddress);
            var response = await _client.SendAsync(request);
            responseStatusCode = (int)response.StatusCode;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        Assert.That(responseStatusCode, Is.EqualTo(429));
    }
}