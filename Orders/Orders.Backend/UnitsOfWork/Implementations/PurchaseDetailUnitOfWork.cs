using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class PurchaseDetailUnitOfWork : GenericUnitOfWork<PurchaseDetail>, IPurchaseDetailUnitOfWork
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public PurchaseDetailUnitOfWork(IGenericRepository<PurchaseDetail> repository, IPurchaseDetailRepository purchaseDetailRepository) : base(repository)
        {
            _purchaseDetailRepository = purchaseDetailRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination) => await _purchaseDetailRepository.GetRecordsNumber(pagination);

        public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination) => await _purchaseDetailRepository.GetAsync(pagination);
    }
}