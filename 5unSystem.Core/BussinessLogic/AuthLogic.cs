using System;
using _5unSystem.Core.DataAccess;
using _5unSystem.Model.DTO;
using _5unSystem.Model.Entities;
using _5unSystem.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _5unSystem.Model.Enum;


namespace _5unSystem.Core.BussinessLogic;

public class AuthLogic
{

    public static LoginResponse AuthProcess(string userName, string password)
    {
        var response = new LoginResponse();
        try
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new Exception(ResponseLoginMessage.USERNAME_OR_PASSWORD_EMPTY);

            var param = new User();
            param.UserName = userName;
            param.Password = password.HashString();

            var user = AuthDataAccess.Login(param);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var key = configuration["Jwt:Key"];
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:ValidityInMinutes"]));
            var refreshTokenExpires = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["Jwt:RefreshTokenValidityInDays"]));
            var audience = configuration["Jwt:Audience"];
            var issuer = configuration["Jwt:Issuer"];

            var Token = GenerateJwtToken(user, key, expires, audience, issuer);
            var RefreshToken = GenerateRefreshToken(user, key, refreshTokenExpires, audience, issuer);

            response.Token = Token;
            response.RefreshToken = RefreshToken;

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private static string GenerateJwtToken(User user, string key, DateTime expires, string audience, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName)
            }),
            Expires = expires,
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    private static string GenerateRefreshToken(User user, string key, DateTime expires, string audience, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("RefreshToken", "true")
            }),
            Expires = expires,
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
