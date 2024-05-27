using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IPurchaseDetailRepository
    {
        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination);
    }
}
