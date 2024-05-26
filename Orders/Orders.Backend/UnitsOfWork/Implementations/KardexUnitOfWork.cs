using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class KardexUnitOfWork : GenericUnitOfWork<PurchaseDetail>, IKardexUnitOfWork
    {
        public KardexUnitOfWork(IGenericRepository<PurchaseDetail> repository) : base(repository)
        {
        }
    }
}