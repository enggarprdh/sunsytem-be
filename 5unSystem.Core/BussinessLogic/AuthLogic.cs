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

            Guid roleID;
            response = AuthDataAccess.Login(param, out roleID);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var key = configuration["Jwt:Key"];
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:ValidityInMinutes"]));
            var refreshTokenExpires = DateTime.UtcNow.AddDays(Convert.ToDouble(configuration["Jwt:RefreshTokenValidityInDays"]));
            var audience = configuration["Jwt:Audience"];
            var issuer = configuration["Jwt:Issuer"];

            GetMenu(roleID, response);
            GenerateJwtToken(response, key, expires, audience, issuer);
            GenerateRefreshToken(response, key, refreshTokenExpires, audience, issuer);

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private static void GenerateJwtToken(LoginResponse response, string key, DateTime expires, string audience, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(key);
        var claims = new List<Claim>();
        claims.Add(new Claim("userName", response.UserName));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        response.Token = tokenHandler.WriteToken(token);
    }
    private static void GenerateRefreshToken(LoginResponse response, string key, DateTime expires, string audience, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, response.UserName),
                new Claim("RefreshToken", "true")
            }),
            Expires = expires,
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        response.RefreshToken = tokenHandler.WriteToken(token);
    }
    private static void GetMenu(Guid roleID, LoginResponse loginResult)
    {
        loginResult.Menu = AuthDataAccess.GetMenu(roleID);
    }
}
