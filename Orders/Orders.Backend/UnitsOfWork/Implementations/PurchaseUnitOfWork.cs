using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class PurchaseUnitOfWork : GenericUnitOfWork<Purchase>, IPurchaseUnitOfWork
    {
        public PurchaseUnitOfWork(IGenericRepository<Purchase> repository) : base(repository)
        {
        }
    }
}
