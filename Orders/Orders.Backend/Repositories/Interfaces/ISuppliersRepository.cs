using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface ISuppliersRepository
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);
    }
}
