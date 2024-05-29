using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class InventoryDetailsRepository : GenericRepository<InventoryDetail>, IInventoryDetailsRepository
    {
        private readonly DataContext _context;

        public InventoryDetailsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            return new ActionResponse<IEnumerable<InventoryDetail>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Include(x => x.Product)
                    .OrderBy(x => x.Product!.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}