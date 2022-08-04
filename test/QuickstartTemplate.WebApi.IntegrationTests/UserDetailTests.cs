using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel.Client;
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
        request.SetBearerToken(MockJwtTokens.GenerateJwtToken(new[] { new Claim("sub", userId) }));
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var readFromString = await response.Content.ReadFromJsonAsync<string>();

        readFromString.Should().BeEquivalentTo(userId);
    }

    // some mocking like MockJwtTokens needed but i have no idea how do do that.
    // [Fact]
    // public async Task Test_HttpContext_User_Identity_Name_fill_correctly_with_SUB_from_OAuth2_Introspection_token()
    // {
    // }
}