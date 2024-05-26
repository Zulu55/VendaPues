using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class PurchaseDetailUnitOfWork : GenericUnitOfWork<Purchase>, IPurchaseDetailUnitOfWork
    {
        public PurchaseDetailUnitOfWork(IGenericRepository<Purchase> repository) : base(repository)
        {
        }
    }
}