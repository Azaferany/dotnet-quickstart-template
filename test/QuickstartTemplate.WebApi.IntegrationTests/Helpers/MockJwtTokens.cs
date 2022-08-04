﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace QuickstartTemplate.WebApi.IntegrationTests.Helpers;


/// <summary>
/// https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
/// </summary>
public static class MockJwtTokens
{
    public static string Issuer { get; } = Guid.NewGuid().ToString();
    public static string Audience { get; } = Guid.NewGuid().ToString();
    public static SecurityKey SecurityKey { get; }
    public static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler s_tokenHandler = new();
    private static readonly RandomNumberGenerator s_rng = RandomNumberGenerator.Create();
    private static readonly byte[] s_key = new byte[32];

    static MockJwtTokens()
    {
        s_rng.GetBytes(s_key);
        SecurityKey = new SymmetricSecurityKey(s_key) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        return s_tokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}