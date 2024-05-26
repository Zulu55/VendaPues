using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IKardexRepository
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);

        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination);
    }
}