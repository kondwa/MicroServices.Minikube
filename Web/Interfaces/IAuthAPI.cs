using Refit;
using Web.Models;

namespace Web.Interfaces
{
    public interface IAuthAPI
    {
        [Get("/")]
        Task<string> GetRootAsync();
        [Post("/login")]
        Task<AuthResponse> LoginAsync([Body] AuthRequest request);
        [Get("/logout")]
        Task LogoutAsync();
    }
}
