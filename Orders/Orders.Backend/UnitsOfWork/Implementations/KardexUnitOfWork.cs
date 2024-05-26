using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class KardexUnitOfWork : GenericUnitOfWork<Kardex>, IKardexUnitOfWork
    {
        private readonly IKardexRepository _kardexRepository;

        public KardexUnitOfWork(IGenericRepository<Kardex> repository, IKardexRepository kardexRepository) : base(repository)
        {
            _kardexRepository = kardexRepository;
        }

        public async Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO) => await _kardexRepository.AddAsync(kardexDTO);

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination) => await _kardexRepository.GetRecordsNumber(pagination);

        public override async Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination) => await _kardexRepository.GetAsync(pagination);
    }
}