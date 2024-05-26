using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public class PurchaseHelper : IPurchaseHelper
    {
        private readonly IProductsUnitOfWork _productsUnitOfWork;
        private readonly IKardexUnitOfWork _kardexUnitOfWork;
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;
        private readonly ISuppliersUnitOfWork _suppliersUnitOfWork;

        public PurchaseHelper(IProductsUnitOfWork productsUnitOfWork, IKardexUnitOfWork kardexUnitOfWork, IPurchaseUnitOfWork purchaseUnitOfWork, ISuppliersUnitOfWork suppliersUnitOfWork)
        {
            _productsUnitOfWork = productsUnitOfWork;
            _kardexUnitOfWork = kardexUnitOfWork;
            _purchaseUnitOfWork = purchaseUnitOfWork;
            _suppliersUnitOfWork = suppliersUnitOfWork;
        }

        public async Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO)
        {
            if (purchaseDTO.PurchaseDetails == null || purchaseDTO.PurchaseDetails.Count == 0)
            {
                return new ActionResponse<bool>
                {
                    Message = "No details in purchase.",
                };
            }

            var responseSupplier = await _suppliersUnitOfWork.GetAsync(purchaseDTO.SupplierId);
            if (!responseSupplier.WasSuccess)
            {
                return new ActionResponse<bool>
                {
                    Message = responseSupplier.Message,
                };
            }

            var purchase = new Purchase
            {
                Date = purchaseDTO.Date,
                Supplier = responseSupplier.Result,
                Remarks = purchaseDTO.Remarks,
                PurchaseDetails = []
            };

            foreach (var purchaseDetail in purchaseDTO.PurchaseDetails)
            {
                var productResponse = await _productsUnitOfWork.GetAsync(purchaseDetail.ProductId);
                if (!productResponse.WasSuccess)
                {
                    return new ActionResponse<bool>
                    {
                        Message = $"Product with Id: {purchaseDetail.ProductId}, not found.",
                    };
                }

                purchase.PurchaseDetails.Add(new PurchaseDetail
                {
                    Cost = purchaseDetail.Cost,
                    Description = productResponse.Result!.Description,
                    Image = productResponse.Result!.MainImage,
                    Name = productResponse.Result!.Name,
                    Product = productResponse.Result,
                    Quantity = purchaseDetail.Quantity,
                    Remarks = purchaseDetail?.Remarks,
                });

                var kardexDTO = new KardexDTO
                {
                    Date = purchase.Date,
                    ProductId = purchaseDetail!.ProductId,
                    KardexType = KardexType.Purchase,
                    Cost = purchaseDetail.Cost,
                    Quantity = purchaseDetail.Quantity
                };

                await _kardexUnitOfWork.AddAsync(kardexDTO);
            }

            var responsePurchase = await _purchaseUnitOfWork.AddAsync(purchase);
            if (!responsePurchase.WasSuccess)
            {
                return new ActionResponse<bool>
                {
                    Message = responsePurchase.Message,
                };
            }

            return new ActionResponse<bool>
            {
                Result = true,
            };
        }
    }
}