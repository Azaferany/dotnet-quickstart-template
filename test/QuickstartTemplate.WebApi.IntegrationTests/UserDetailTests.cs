using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QuickstartTemplate.WebApi.IntegrationTests.Helpers;

namespace QuickstartTemplate.WebApi.IntegrationTests;

public class UserDetailTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _customWebApplicationFactory;

    public UserDetailTests(CustomWebApplicationFactory customWebApplicationFactory)
    {
        _customWebApplicationFactory = customWebApplicationFactory;
    }

    [Fact]
    public async Task Test_HttpContext_User_Identity_Name_fill_correctly_with_SUB_from_JWT_token()
    {
        //
        var factory = _customWebApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddControllers()
                    .AddApplicationPart(GetType().Assembly))); // add TestController to app Controller

        var client = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "v1/Test/UserDetail");

        var userId = "64624";
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer",MockJwtTokens.GenerateJwtToken(new[] { new Claim("sub", userId) }));

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var readFromString = await response.Content.ReadFromJsonAsync<string>();

        readFromString.Should().BeEquivalentTo(userId);
    }

    [Fact]
    public async Task Test_Default_Culture_On_RequestLocalization()
    {
        var factory = _customWebApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddControllers()
                    .AddApplicationPart(GetType().Assembly))); // add TestController to app Controller
        // Arrange
        var client = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "v1/Test/Culture");

        // Act
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Test-en");
    }

    [Theory]
    [InlineData("en-US", "Test-en")]
    [InlineData("fa-IR", "Test-fa")]
    public async Task Test_Cultures_On_RequestLocalization(string acceptLanguageHeader, string expectedValue)
    {
        var factory = _customWebApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddControllers()
                    .AddApplicationPart(GetType().Assembly))); // add TestController to app Controller
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguageHeader));

        var request = new HttpRequestMessage(HttpMethod.Get, "v1/Test/Culture");

        // Act
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(expectedValue);
    }
}
