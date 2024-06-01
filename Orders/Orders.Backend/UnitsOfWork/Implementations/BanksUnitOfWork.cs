using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class BanksUnitOfWork : GenericUnitOfWork<Bank>, IBanksUnitOfWork
    {
        private readonly IBanksRepository _banksRepository;

        public BanksUnitOfWork(IGenericRepository<Bank> repository, IBanksRepository banksRepository) : base(repository)
        {
            _banksRepository = banksRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _banksRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination) => await _banksRepository.GetAsync(pagination);

        public async Task<IEnumerable<Bank>> GetComboAsync() => await _banksRepository.GetComboAsync();
    }
}