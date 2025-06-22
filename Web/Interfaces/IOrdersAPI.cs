using Refit;
using Web.Models;

namespace Web.Interfaces
{
    public interface IOrdersAPI
    {
        [Get("/")]
        Task<string> GetRootAsync();
        [Get("/orders")]
        Task<OrderData> GetOrdersAsync();
    }
}
