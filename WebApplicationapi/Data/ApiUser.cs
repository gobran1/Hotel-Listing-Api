using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationapi.Data
{
  public class ApiUser : IdentityUser
  {
    [StringLength(maximumLength: 20)]
    public virtual string FirstName { get; set; }

    [StringLength(maximumLength: 20)]
    public virtual string LastName { get; set; }
  }
}
