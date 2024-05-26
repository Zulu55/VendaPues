using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IKardexUnitOfWork
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);
    }
}
