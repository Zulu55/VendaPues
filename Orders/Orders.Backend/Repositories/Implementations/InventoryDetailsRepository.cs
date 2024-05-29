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

        public override async Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail)
        {
            var currentInventoryDetail = await _context.InventoryDetails.FindAsync(inventoryDetail.Id);
            if (currentInventoryDetail == null)
            {
                return new ActionResponse<InventoryDetail>
                {
                    WasSuccess = false,
                    Message = "Detalle de inventario no existe."
                };
            }

            currentInventoryDetail.Cost = inventoryDetail.Cost;
            currentInventoryDetail.Count1 = inventoryDetail.Count1;
            currentInventoryDetail.Count2 = inventoryDetail.Count2;
            currentInventoryDetail.Count3 = inventoryDetail.Count3;
            currentInventoryDetail.Adjustment = inventoryDetail.Adjustment;

            _context.Update(currentInventoryDetail);
            await _context.SaveChangesAsync();

            return new ActionResponse<InventoryDetail>
            {
                WasSuccess = true,
                Result = currentInventoryDetail
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
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

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
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