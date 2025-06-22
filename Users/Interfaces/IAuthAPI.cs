using Refit;
using Users.Types;

namespace Users.Interfaces
{
    public interface IAuthAPI
    {
        [Post("/login")]
        Task<AuthResponse> LoginAsync([Body] AuthRequest request);
    }
}
