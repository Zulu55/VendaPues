using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class KardexRepository : GenericRepository<Kardex>, IKardexRepository
    {
        private readonly DataContext _context;

        public KardexRepository(DataContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO)
        {
            var product = await _context.Products.FindAsync(kardexDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<bool>
                {
                    Message = $"Kardex - Product with Id: {kardexDTO.ProductId}, not found.",
                };
            }

            var kardex = new Kardex
            {
                Date = kardexDTO.Date,
                ProductId = kardexDTO.ProductId,
                KardexType = kardexDTO.KardexType,
                Cost = kardexDTO.Cost,
                Quantity = kardexDTO.Quantity,
            };
            _context.Kardex.Add(kardex);
            await _context.SaveChangesAsync();

            var kardexForProdcut = await _context.Kardex
                .Where(x => x.ProductId == kardexDTO.ProductId)
                .OrderBy(x => x.Date)
                .ToListAsync();

            await ReKardexAsync(kardexForProdcut);

            return new ActionResponse<bool>
            {
                WasSuccess = true,
            };
        }

        private async Task ReKardexAsync(List<Kardex> kardexForProduct)
        {
            Kardex? previousKardex = null;
            foreach (var kardex in kardexForProduct)
            {
                switch (kardex.KardexType)
                {
                    case KardexType.Purchase:
                        if (previousKardex == null)
                        {
                            kardex.Balance = kardex.Quantity;
                            kardex.AverageCost = kardex.Cost;
                        }
                        else
                        {
                            kardex.Balance = kardex.Quantity + previousKardex.Balance;
                            kardex.AverageCost = ((decimal)kardex.Quantity * kardex.Cost + (decimal)previousKardex.Quantity * previousKardex.AverageCost) / (decimal)kardex.Balance;
                        }
                        break;

                    case KardexType.Order:
                        kardex.Balance -= kardex.Quantity;
                        if (previousKardex == null)
                        {
                            kardex.AverageCost = 0;
                        }
                        else
                        {
                            kardex.AverageCost = previousKardex.AverageCost;
                        }
                        break;
                }
                previousKardex = kardex;
            }

            var product = await _context.Products.FindAsync(previousKardex!.ProductId);
            if (product != null)
            {
                product.Cost = previousKardex.AverageCost;
                product.Stock = previousKardex.Balance;
            }
            await _context.SaveChangesAsync();
        }
    }
}