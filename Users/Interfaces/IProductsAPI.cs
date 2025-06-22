using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Users.Interfaces
{
    public interface IProductsAPI
    {
        [Get("/products")]
        Task<IEnumerable<string>> GetProductsAsync();
    }
}
