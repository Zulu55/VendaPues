using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class InventoryDetailsUnitOfWork : GenericUnitOfWork<InventoryDetail>, IInventoryDetailsUnitOfWork
    {
        private readonly IInventoryDetailsRepository _inventoryDetailsRepository;

        public InventoryDetailsUnitOfWork(IGenericRepository<InventoryDetail> repository, IInventoryDetailsRepository inventoryDetailsRepository) : base(repository)
        {
            _inventoryDetailsRepository = inventoryDetailsRepository;
        }

        public override async Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail model) => await _inventoryDetailsRepository.UpdateAsync(model);

        public async Task<ActionResponse<int>> GetRecordsNumberCount1Async(PaginationDTO pagination) => await _inventoryDetailsRepository.GetRecordsNumberCount1Async(pagination);

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount1Async(PaginationDTO pagination) => await _inventoryDetailsRepository.GetCount1Async(pagination);
    }
}