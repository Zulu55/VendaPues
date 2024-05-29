using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IInventoryDetailsUnitOfWork
    {
        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetAsync(PaginationDTO pagination);
    }
}
