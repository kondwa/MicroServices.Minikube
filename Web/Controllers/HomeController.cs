using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController(IAuthAPI authAPI, IUsersAPI usersAPI, IOrdersAPI ordersAPI, IProductsAPI productsAPI) : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<string> status = [
                (await authAPI.GetRootAsync()).ToString(),
                (await usersAPI.GetRootAsync()).ToString(),
                (await ordersAPI.GetRootAsync()).ToString(),
                (await productsAPI.GetRootAsync()).ToString(),
            ];
            return View(status);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
