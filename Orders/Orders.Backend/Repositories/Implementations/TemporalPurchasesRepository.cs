using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class TemporalPurchasesRepository : GenericRepository<TemporalPurchase>, ITemporalPurchasesRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public TemporalPurchasesRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context = context;
            _usersRepository = usersRepository;
        }

        public override async Task<ActionResponse<TemporalPurchase>> GetAsync(int id)
        {
            var temporalPurchase = await _context.TemporalPurchases
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (temporalPurchase == null)
            {
                return new ActionResponse<TemporalPurchase>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            return new ActionResponse<TemporalPurchase>
            {
                WasSuccess = true,
                Result = temporalPurchase
            };
        }

        public async Task<ActionResponse<TemporalPurchaseDTO>> AddFullAsync(string email, TemporalPurchaseDTO temporalPurchaseDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalPurchaseDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message = "Producto no existe"
                };
            }

            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message = "Usuario no existe"
                };
            }

            var temporalPurchase = new TemporalPurchase
            {
                Product = product,
                Quantity = temporalPurchaseDTO.Quantity,
                Remarks = temporalPurchaseDTO.Remarks,
                User = user,
                Cost = temporalPurchaseDTO.Cost,
            };

            try
            {
                _context.Add(temporalPurchase);
                await _context.SaveChangesAsync();
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = true,
                    Result = temporalPurchaseDTO
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<TemporalPurchaseDTO>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<IEnumerable<TemporalPurchase>>> GetAsync(string email)
        {
            var temporalPurchases = await _context.TemporalPurchases
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .Where(x => x.User!.Email == email)
                .ToListAsync();

            return new ActionResponse<IEnumerable<TemporalPurchase>>
            {
                WasSuccess = true,
                Result = temporalPurchases
            };
        }

        public async Task<ActionResponse<int>> GetCountAsync(string email)
        {
            var count = await _context.TemporalPurchases
                .Where(x => x.User!.Email == email)
                .SumAsync(x => x.Quantity);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<TemporalPurchase>> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO)
        {
            var currentTemporalOrder = await _context.TemporalPurchases.FirstOrDefaultAsync(x => x.Id == temporalPurchaseDTO.Id);
            if (currentTemporalOrder == null)
            {
                return new ActionResponse<TemporalPurchase>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            currentTemporalOrder!.Remarks = temporalPurchaseDTO.Remarks;
            currentTemporalOrder.Quantity = temporalPurchaseDTO.Quantity;
            currentTemporalOrder.Cost = temporalPurchaseDTO.Cost;

            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return new ActionResponse<TemporalPurchase>
            {
                WasSuccess = true,
                Result = currentTemporalOrder
            };
        }
    }
}