using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class NewsUnitOfWork : GenericUnitOfWork<NewsArticle>, INewsUnitOfWork
    {
        private readonly INewsRepository _newsRepository;

        public NewsUnitOfWork(IGenericRepository<NewsArticle> repository, INewsRepository newsRepository) : base(repository)
        {
            _newsRepository = newsRepository;
        }

        public override async Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle model) => await _newsRepository.UpdateAsync(model);

        public override async Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle model) => await _newsRepository.AddAsync(model);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _newsRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination) => await _newsRepository.GetAsync(pagination);
    }
}