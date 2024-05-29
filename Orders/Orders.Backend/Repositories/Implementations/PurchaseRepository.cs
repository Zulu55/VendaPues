using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
    {
        private readonly DataContext _context;

        public PurchaseRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Purchases
                .Include(x => x.Supplier)
                .AsQueryable();
            int recordsNumber = await queryable.CountAsync();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Supplier!.SupplierName.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Purchases
                .Include(x => x.Supplier)
                .Include(x => x.PurchaseDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Supplier!.SupplierName.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
            }

            return new ActionResponse<IEnumerable<Purchase>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderByDescending(x => x.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<Purchase>> GetAsync(int id)
        {
            var purchase = await _context.Purchases
                .Include(x => x.Supplier!)
                .Include(x => x.PurchaseDetails!)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (purchase == null)
            {
                return new ActionResponse<Purchase>
                {
                    Message = "Compra no existe."
                };
            }

            return new ActionResponse<Purchase>
            {
                WasSuccess = true,
                Result = purchase
            };
        }
    }
}