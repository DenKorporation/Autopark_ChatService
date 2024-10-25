using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.IntegrationTests.JwtTokenGeneration;

public static class JwtTokenProvider
{
    public static JwtSecurityTokenHandler JwtSecurityTokenHandler { get; } = new JwtSecurityTokenHandler();
    public static string Issuer { get; } = "test.auth";

    public static SecurityKey SecurityKey { get; } =
        new SymmetricSecurityKey("This_is_a_super_secure_key_and_you_know_it"u8.ToArray());

    public static SigningCredentials SigningCredentials { get; } =
        new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
}
