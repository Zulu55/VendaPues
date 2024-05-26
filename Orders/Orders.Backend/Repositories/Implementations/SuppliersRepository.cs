using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.Repositories.Implementations
{
    public class SuppliersRepository : GenericRepository<Supplier>, ISuppliersRepository
    {
        public SuppliersRepository(DataContext context) : base(context)
        {
        }
    }
}
