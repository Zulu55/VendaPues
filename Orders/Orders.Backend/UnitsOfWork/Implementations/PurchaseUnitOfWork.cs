using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class PurchaseUnitOfWork : GenericUnitOfWork<Purchase>, IPurchaseUnitOfWork
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseUnitOfWork(IGenericRepository<Purchase> repository, IPurchaseRepository purchaseRepository) : base(repository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _purchaseRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<Purchase>> GetAsync(int id) => await _purchaseRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination) => await _purchaseRepository.GetAsync(pagination);
    }
}