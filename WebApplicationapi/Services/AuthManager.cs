using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplicationapi.Data;
using WebApplicationapi.Models;

namespace WebApplicationapi.Services
{
  public class AuthManager : IAuthManager
  {
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _user;

    public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
    }

    public async Task<string> CreateToken()
    {
      var signingCredentials = GetSigningCredentials();
      var claims = await GetClaims();
      var token = GenerateToken(signingCredentials, claims);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
      var key = _configuration.GetSection("JWT").GetSection("key").Value;
      var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<IList<Claim>> GetClaims()
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, _user.UserName)
      };

      var roles = await _userManager.GetRolesAsync(_user);
      foreach (var role in roles)
      {
        claims.Add(
          new Claim(ClaimTypes.Role, role)
        );
      }

      return claims;
    }

    private JwtSecurityToken GenerateToken(SigningCredentials signingCredentials, IList<Claim> claims)
    {
      var lifeTime = DateTime.Now.AddMinutes(
        double.Parse(_configuration.GetSection("JWT").GetSection("lifetime").Value)
      );

      return new JwtSecurityToken
      (
        _configuration.GetSection("JWT").GetSection("issuer").Value,
        claims: claims,
        signingCredentials: signingCredentials,
        expires: lifeTime
      );
    }

    public async Task<bool> ValidateUser(LoginUserDTO userDto)
    {
      _user = await _userManager.FindByNameAsync(userDto.Email);
      return (_user != null) && await _userManager.CheckPasswordAsync(_user, userDto.Password);
    }
  }
}
