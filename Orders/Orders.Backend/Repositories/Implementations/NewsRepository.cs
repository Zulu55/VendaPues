using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class NewsRepository : GenericRepository<NewsArticle>, INewsRepository
    {
        private readonly DataContext _context;

        public NewsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<NewsArticle>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Title)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}