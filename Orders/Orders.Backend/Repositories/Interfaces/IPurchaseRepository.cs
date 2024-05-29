using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<ActionResponse<Purchase>> AddAsync(Purchase entity);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<Purchase>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination);
    }
}
