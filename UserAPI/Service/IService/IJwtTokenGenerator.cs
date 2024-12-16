using UserAPI.Models;

namespace UserAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
