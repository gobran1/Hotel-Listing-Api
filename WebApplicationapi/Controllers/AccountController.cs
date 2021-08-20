using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using WebApplicationapi.Data;
using WebApplicationapi.Models;
using WebApplicationapi.Services;
using ILogger = Serilog.ILogger; 

namespace hotellisting.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : Controller
  {
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private UserManager<ApiUser> _userManager;
    private IAuthManager _authManager;

    public AccountController(ILogger<AccountController> logger, IMapper mapper, UserManager<ApiUser> userManager,
      IAuthManager authManager)
    {
      _logger = logger;
      _mapper = mapper;
      _userManager = userManager;
      _authManager = authManager;
    }


    [HttpPost("register")]
    [ProducesResponseType(202)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var user = _mapper.Map<ApiUser>(userDto);
        user.UserName = userDto.Email;
        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }

          return BadRequest(ModelState);
        }

        await _userManager.AddToRolesAsync(user, userDto.Roles);

        return Accepted();
      }
      catch (Exception e)
      {
        _logger.LogError(e, $"error occured in {nameof(Register)} Method");
        return Problem("internal server Error,try again later", statusCode: 500);
      }
    }

    [HttpPost("login")]
    [ProducesResponseType(202)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        if (!await _authManager.ValidateUser(userDto))
        {
          return Unauthorized();
        }

        return Accepted(new
          {
            token = await _authManager.CreateToken()
          }
        );
      }
      catch (Exception e)
      {
        _logger.LogError(e, $"error occured in {nameof(Login)} Method");
        return Problem("internal server Error,try again later", statusCode: 500);
      }
    }
  }
}
