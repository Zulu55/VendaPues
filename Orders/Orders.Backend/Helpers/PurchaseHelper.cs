using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public class PurchaseHelper : IPurchaseHelper
    {
        public Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO)
        {
            throw new NotImplementedException();
        }
    }
}
