using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface ISuppliersUnitOfWork
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);
    }
}
