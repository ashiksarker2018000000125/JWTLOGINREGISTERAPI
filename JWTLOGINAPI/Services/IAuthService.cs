using JWTLOGINAPI.Models;

namespace JWTLOGINAPI.Services
{
    public interface IAuthService
    {
        Task<string> GenerateTokenString(LoginUser user);
        Task<bool> Login(LoginUser user);
        Task<bool> RegisterUser(RegisterUser user);
    }
}
