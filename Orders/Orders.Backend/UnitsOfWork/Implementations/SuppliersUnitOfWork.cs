using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class SuppliersUnitOfWork : GenericUnitOfWork<Supplier>, ISuppliersUnitOfWork
    {
        public SuppliersUnitOfWork(IGenericRepository<Supplier> repository) : base(repository)
        {
        }
    }
}
