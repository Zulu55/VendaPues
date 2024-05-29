using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class InventoriesUnitOfWork : GenericUnitOfWork<Inventory>, IInventoriesUnitOfWork
    {
        private readonly IInventoriesRepository _inventoriesRepository;

        public InventoriesUnitOfWork(IGenericRepository<Inventory> repository, IInventoriesRepository inventoriesRepository) : base(repository)
        {
            _inventoriesRepository = inventoriesRepository;
        }

        public async Task<ActionResponse<bool>> FinishCount1(int id) => await _inventoriesRepository.FinishCount1(id);

        public override async Task<ActionResponse<Inventory>> GetAsync(int id) => await _inventoriesRepository.GetAsync(id);

        public override async Task<ActionResponse<Inventory>> AddAsync(Inventory model) => await _inventoriesRepository.AddAsync(model);

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination) => await _inventoriesRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination) => await _inventoriesRepository.GetAsync(pagination);
    }
}