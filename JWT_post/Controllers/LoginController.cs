using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT_post.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWT_post.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    public LoginController(IConfiguration config)
    {
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult Login([FromBody] UserLogin userLogin)
    {
        var user = Authenticate(userLogin);
        if (user == null) return NotFound("user not found");
        var token = GenerateToken(user);

        return Ok(token);
    }

    private string GenerateToken(UserModel user)
    {
        if (user.Token != null && ValidateToken(user.Token))
        {
            return user.Token;
        }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        user.Token = generatedToken;
        return generatedToken;
    }

    private bool ValidateToken(string token)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtTokenHandler.ReadToken(token) as JwtSecurityToken;
        
        var expDate = securityToken?.ValidTo;
        return expDate > DateTime.UtcNow;
    }

    private UserModel? Authenticate(UserLogin userLogin)
    {
        var currentUser = UserConstants.Users
            .FirstOrDefault(x => 
                x.Username.ToLower() == userLogin.Username.ToLower()
                && x.Password == userLogin.Password
            );
        if (currentUser != null)
        {
            return currentUser;
        }

        return null;
    }
}