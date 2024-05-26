using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IKardexRepository
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);
    }
}