using System.Threading.Tasks;
using WebApplicationapi.Models;

namespace WebApplicationapi.Services
{
  public interface IAuthManager
  {
    Task<bool> ValidateUser(LoginUserDTO userDto);
    Task<string> CreateToken();
  }
}
