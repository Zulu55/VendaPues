using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class PurchaseDetailRepository : GenericRepository<PurchaseDetail>, IPurchaseDetailRepository
    {
        private readonly DataContext _context;

        public PurchaseDetailRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination)
        {
            var queryable = _context.PurchaseDetails.AsQueryable();
            int recordsNumber = await queryable.CountAsync();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.PurchaseDetails.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }

            return new ActionResponse<IEnumerable<PurchaseDetail>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderByDescending(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}