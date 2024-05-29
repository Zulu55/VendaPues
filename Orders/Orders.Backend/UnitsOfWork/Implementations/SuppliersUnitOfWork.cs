using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class SuppliersUnitOfWork : GenericUnitOfWork<Supplier>, ISuppliersUnitOfWork
    {
        private readonly ISuppliersRepository _suppliersRepository;

        public SuppliersUnitOfWork(IGenericRepository<Supplier> repository, ISuppliersRepository suppliersRepository) : base(repository)
        {
            this._suppliersRepository = suppliersRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination) => await _suppliersRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination) => await _suppliersRepository.GetAsync(pagination);

        public async Task<IEnumerable<Supplier>> GetComboAsync() => await _suppliersRepository.GetComboAsync();

        public override async Task<ActionResponse<Supplier>> GetAsync(int id) => await _suppliersRepository.GetAsync(id);
    }
}
