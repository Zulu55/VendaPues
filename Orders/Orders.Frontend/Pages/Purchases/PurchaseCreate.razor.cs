using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Purchases
{
    public partial class PurchaseCreate
    {
        private TemporalPurchaseDTO temporalPurchaseDTO = new() { Date = DateTime.Now };
        private List<Supplier>? suppliers;
        private Supplier selectedSupplier = new();
        private List<Product>? products;
        private Product selectedProduct = new();
        private MudTable<TemporalPurchase> table = new();
        private List<TemporalPurchase> temporalPurchases = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalPurchases";
        private readonly int[] pageSizeOptions = { 5, 10 };
        private int totalRecords = 0;
        private bool loading;
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        public List<TemporalPurchase>? TemporalPurchases { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSuppliersAsync();
            await LoadPrductsAsync();
            await LoadTemporalPurchasesAsync();
        }

        private async Task LoadTemporalPurchasesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<TemporalPurchase>>($"/api/TemporalPurchases/my");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            foreach (var item in responseHttp.Response!)
            {
                temporalPurchases.Add(new TemporalPurchase
                {
                    Cost = item.Cost,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });
            }
        }

        private async Task LoadPrductsAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Product>>($"/api/products/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }
            products = responseHttp.Response;
        }

        private async Task<TableData<TemporalPurchase>> LoadListAsync(TableState state)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await Repository.GetAsync<List<TemporalPurchase>>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return new TableData<TemporalPurchase> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<TemporalPurchase> { Items = [], TotalItems = 0 };
            }

            sumQuantity = responseHttp.Response.Sum(x => x.Quantity);
            sumValue = responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);

            return new TableData<TemporalPurchase>
            {
                Items = responseHttp.Response,
                TotalItems = responseHttp.Response.Count
            };
        }

        private async Task DeleteAsync(int temporalPurchaseId)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres borrar el registro?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<TemporalPurchase>($"api/temporalPurchases/{temporalPurchaseId}");

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            await table.ReloadServerData();
            ShowToast("Ok", SweetAlertIcon.Success, "Producto eliminado de la compra.");
        }

        private async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            temporalPurchaseDTO.Date = (DateTime)date;
        }

        private async Task AddProductAsync()
        {
            if (selectedProduct.Id == 0)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes seleccionar un producto.");
                return;
            }

            if (temporalPurchaseDTO.Quantity <= 0)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes ingresar una cantidad mayor que cero.");
                return;
            }

            if (temporalPurchaseDTO.Cost <= 0)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes ingresar un costo mayor que cero.");
                return;
            }

            var responseHttp = await Repository.PostAsync<TemporalPurchaseDTO>("/api/TemporalPurchases/full", temporalPurchaseDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            temporalPurchases.Add(new TemporalPurchase
            {
                Cost = temporalPurchaseDTO.Cost,
                ProductId = temporalPurchaseDTO.ProductId,
                Quantity = temporalPurchaseDTO.Quantity,
                Remarks = temporalPurchaseDTO.RemarksDetail,
            });

            selectedProduct = new Product();
            temporalPurchaseDTO.Quantity = 1;
            temporalPurchaseDTO.Cost = 0;
            await table.ReloadServerData();
            ShowToast("Ok", SweetAlertIcon.Success, "Producto agregado a la compra.");
        }

        private async Task SavePurchaseAsync()
        {
            if (selectedSupplier.Id == 0)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes seleccionar un proveedor.");
                return;
            }

            if (sumQuantity <= 0)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes agregar al menos un producto en la compra.");
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres registrar esta compra?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var purchaseDTO = new PurchaseDTO
            {
                Date = temporalPurchaseDTO.Date.ToUniversalTime(),
                Remarks = temporalPurchaseDTO.RemarksGeneral,
                SupplierId = temporalPurchaseDTO.SupplierId,
                PurchaseDetails = []
            };

            foreach (var item in temporalPurchases)
            {
                purchaseDTO.PurchaseDetails.Add(new PurchaseDetailDTO
                {
                    Cost = item.Cost,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks
                });
            }

            var responseHttp = await Repository.PostAsync<PurchaseDTO>("/api/purchases/full", purchaseDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            selectedSupplier = new Supplier();
            temporalPurchases.Clear();
            temporalPurchaseDTO.RemarksGeneral = string.Empty;
            NavigationManager.NavigateTo("/purchases");
            ShowToast("Ok", SweetAlertIcon.Success, "Compra agregada con exito.");
        }

        private void SuplierChanged(Supplier supplier)
        {
            selectedSupplier = supplier;
            temporalPurchaseDTO.SupplierId = supplier.Id;
        }

        private void ProductChanged(Product product)
        {
            selectedProduct = product;
            temporalPurchaseDTO.ProductId = product.Id;
            temporalPurchaseDTO.Cost = product.Cost;
            temporalPurchaseDTO.Quantity = 1;
        }

        private async Task LoadSuppliersAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Supplier>>($"/api/suppliers/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }
            suppliers = responseHttp.Response;
        }

        private async Task<IEnumerable<Supplier>> SearchSupplierAsync(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return suppliers!;
            }

            return suppliers!
                .Where(x => x.SupplierName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Product>> SearchProductAsync(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return products!;
            }

            return products!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private void ShowToast(string title, SweetAlertIcon iconMessage, string message)
        {
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            _ = toast.FireAsync(title, message, iconMessage);
        }
    }
}