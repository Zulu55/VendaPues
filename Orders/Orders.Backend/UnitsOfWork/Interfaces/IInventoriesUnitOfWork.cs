using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IInventoriesUnitOfWork
    {
        Task<ActionResponse<Inventory>> GetAsync(int id);

        Task<ActionResponse<Inventory>> AddAsync(Inventory inventory);

        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination);
    }
}
