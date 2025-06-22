using Refit;
using Web.Models;

namespace Web.Interfaces
{
    public interface IUsersAPI
    {
        [Get("/")]
        Task<string> GetRootAsync();
        [Get("/users")]
        Task<UserData> GetUserDataAsync();
        [Get("/orders")]
        Task<OrderData> GetOrdersAsync();
        [Get("/products")]
        Task<ProductData> GetProductsAsync();
    }
}
