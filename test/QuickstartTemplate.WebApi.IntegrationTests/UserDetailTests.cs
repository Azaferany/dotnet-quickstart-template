using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using IdentityModel.Jwk;
using Microsoft.Extensions.DependencyInjection;
using QuickstartTemplate.WebApi.IntegrationTests.Helpers;
using RichardSzalay.MockHttp;

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
    [Fact]
    public async Task Test_HttpContext_User_Identity_Name_fill_correctly_with_SUB_from_OAuth2_Introspection_token()
    {
        var userId = "12345";

        var factory = _customWebApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                {
                    services.AddControllers()
                            .AddApplicationPart(GetType().Assembly);

                    services.AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName)
                            .ConfigurePrimaryHttpMessageHandler(_ =>
                            {
                                var mockHttp = new MockHttpMessageHandler();

                                mockHttp.When($"*/introspect")
                                        .Respond(JsonContent.Create(new
                                        {
                                            iss = "https://demo.duendesoftware.com",
                                            client_id = "PayPingWebApp",
                                            sub = userId,
                                            name = "TestName",
                                            active = true,
                                            scope =
                                                "QuickstartTemplate:admin QuickstartTemplate:read QuickstartTemplate:write"
                                        }));

                                mockHttp.When($"*/openid-configuration")
                                        .Respond(JsonContent.Create(new
                                        {
                                            issuer = "https://demo.duendesoftware.com",
                                            jwks_uri =
                                                "https://demo.duendesoftware.com/.well-known/openid-configuration/jwks",
                                            authorization_endpoint =
                                                "https://demo.duendesoftware.com/connect/authorize",
                                            token_endpoint = "https://demo.duendesoftware.com/connect/token",
                                            userinfo_endpoint = "https://demo.duendesoftware.com/connect/userinfo",
                                            introspection_endpoint =
                                                "https://demo.duendesoftware.com/connect/introspect",
                                            scopes_supported = new List<string>()
                                            {
                                                "QuickstartTemplate:admin",
                                                "QuickstartTemplate:read",
                                                "QuickstartTemplate:write"
                                            },
                                            claims_supported = new List<string>() { "sub", "name" }
                                        }));
                                mockHttp.When("*/jwks").Respond(JsonContent.Create(new JsonWebKeySet
                                {
                                    Keys = new List<JsonWebKey>() { new JsonWebKey() { Alg = "test" } }
                                }));
                                return mockHttp;
                            });
                }
            ));

        var client = factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "v1/Test/UserDetail");

        request.SetBearerToken("testtoken");

        var response = await client.SendAsync(request);

        var readFromString = await response.Content.ReadFromJsonAsync<string>();

        readFromString.Should().BeEquivalentTo(userId);
    }
}
