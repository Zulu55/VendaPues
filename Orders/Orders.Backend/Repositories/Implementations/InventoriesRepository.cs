using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class InventoriesRepository : GenericRepository<Inventory>, IInventoriesRepository
    {
        private readonly DataContext _context;

        public InventoriesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ActionResponse<bool>> FinishCount1(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Inventario no existe"
                };
            }

            inventory.Count1Finish = true;
            _context.Update(inventory);
            await _context.SaveChangesAsync();
            return new ActionResponse<bool> { WasSuccess = true };
        }

        public override async Task<ActionResponse<Inventory>> GetAsync(int id)
        {
            var inventory = await _context.Inventories
                 .Include(x => x.InventoryDetails!)
                 .FirstOrDefaultAsync(x => x.Id == id);

            if (inventory == null)
            {
                return new ActionResponse<Inventory>
                {
                    WasSuccess = false,
                    Message = "Inventario no existe"
                };
            }

            return new ActionResponse<Inventory>
            {
                WasSuccess = true,
                Result = inventory
            };
        }

        public override async Task<ActionResponse<Inventory>> AddAsync(Inventory inventory)
        {
            inventory.InventoryDetails = [];
            inventory.Date = inventory.Date.ToUniversalTime();
            var products = await _context.Products.ToListAsync();
            foreach (var product in products)
            {
                inventory.InventoryDetails.Add(new InventoryDetail
                {
                    Cost = product.Cost,
                    ProductId = product.Id,
                    Stock = product.Stock,
                });
            }

            await base.AddAsync(inventory);
            return new ActionResponse<Inventory>
            {
                WasSuccess = true,
                Result = inventory,
            };
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Inventories.AsQueryable();
            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Inventories.AsQueryable();

            return new ActionResponse<IEnumerable<Inventory>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderByDescending(x => x.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}