using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplicationapi.Models
{
  public class LoginUserDTO
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 5,
        ErrorMessage = "password should be at less 5 character and at most 30 character")]
    public string Password { get; set; }
  }

  public class UserDTO : LoginUserDTO
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Phone] public string PhoneNumber { get; set; }

    [Required] public ICollection<string> Roles { get; set; }
  }
}
