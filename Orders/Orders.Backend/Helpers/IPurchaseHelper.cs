using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public interface IPurchaseHelper
    {
        Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO, string email);
    }
}