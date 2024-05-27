using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IPurchaseUnitOfWork
    {
        Task<ActionResponse<Purchase>> AddAsync(Purchase entity);

        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<Purchase>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination);
    }
}
