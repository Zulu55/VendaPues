using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public interface IOrdersHelper
    {
        Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO);
    }
}