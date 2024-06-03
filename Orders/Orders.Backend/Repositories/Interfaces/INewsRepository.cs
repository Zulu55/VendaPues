using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface INewsRepository
    {
        Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle);

        Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination);
    }
}