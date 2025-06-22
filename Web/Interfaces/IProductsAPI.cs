using Refit;
using Web.Models;

namespace Web.Interfaces
{
    public interface IProductsAPI
    {
        [Get("/")]
        Task<string> GetRootAsync();
        [Get("/products")]
        Task<ProductData> GetProductsAsync();
    }
}
