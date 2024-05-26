using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.Repositories.Implementations
{
    public class KardexRepository : GenericRepository<PurchaseDetail>, IKardexRepository
    {
        public KardexRepository(DataContext context) : base(context)
        {
        }
    }
}
