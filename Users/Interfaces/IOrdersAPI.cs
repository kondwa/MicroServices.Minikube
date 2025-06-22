using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Users.Interfaces
{
    public interface IOrdersAPI
    {
        [Get("/orders")]
        Task<IEnumerable<string>> GetOrdersAsync();
    }
}
