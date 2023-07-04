using JWT_post.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_post.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public ActionResult Register([FromBody] UserLogin userLogin)
    {
        if (UserConstants.Users.Any(x => String.Equals(x.Username, userLogin.Username, 
                StringComparison.CurrentCultureIgnoreCase)))
            return Conflict("User already exists");
        var user = new UserModel {Username = userLogin.Username, Password = userLogin.Password, Role = "User"};
        UserConstants.Users.Add(user);
        return Ok("User registered");
    }
}