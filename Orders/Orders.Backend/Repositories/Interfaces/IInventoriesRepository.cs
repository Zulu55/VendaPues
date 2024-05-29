using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IInventoriesRepository
    {
        Task<ActionResponse<bool>> FinishCount1Async(int id);

        Task<ActionResponse<bool>> FinishCount2Async(int id);

        Task<ActionResponse<bool>> FinishCount3Async(int id);

        Task<ActionResponse<Inventory>> GetAsync(int id);
        
        Task<ActionResponse<Inventory>> AddAsync(Inventory inventory);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination);
    }
}
