using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;

namespace Web.Controllers
{
    [Authorize]
    public class SecureController(IUsersAPI usersAPI) : Controller
    {
        public async Task<IActionResult> UserDataAsync()
        {
            var userData = await usersAPI.GetUserDataAsync();
            return View(userData);
        }
        public async Task<IActionResult> OrdersDataAsync()
        {
            var ordersData = await usersAPI.GetOrdersAsync();
            return View(ordersData);
        }
        public async Task<IActionResult> ProductsDataAsync()
        {
            var productsData = await usersAPI.GetProductsAsync();
            return View(productsData);
        }
    }
}
