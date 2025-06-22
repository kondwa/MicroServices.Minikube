using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController(IAuthAPI authAPI) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            if (ModelState.IsValid) { 
                var response = await authAPI.LoginAsync(request);
                if (!string.IsNullOrEmpty(response.Token))
                {
                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);
                    var claims = jwt.Claims.ToList();
                    claims.Add(new Claim("access_token", response.Token));
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { 
                        IsPersistent = true 
                    });
                    Response.Cookies.Append("token", response.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await authAPI.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("token");
            return RedirectToAction("Login");
        }
    }
}
