using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IInventoryDetailsRepository
    {
        Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail);

        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetAsync(PaginationDTO pagination);
    }
}