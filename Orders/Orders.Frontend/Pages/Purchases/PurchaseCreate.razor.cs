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
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalPurchases";
        private readonly int[] pageSizeOptions = { 5, 10 };
        private int totalRecords = 0;
        private bool loading;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        public List<TemporalPurchase>? TemporalPurchases { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSuppliersAsync();
            await LoadPrductsAsync();
        }

        private async Task LoadPrductsAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Product>>($"/api/products/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
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
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
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

                var mensajeError = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                return;
            }

            await table.ReloadServerData();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Producto eliminado de la compra.");
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

        private async Task SavePurchaseAsync()
        {
            //userDTO.UserType = UserType.User;
            //userDTO.UserName = userDTO.Email;

            //if (IsAdmin)
            //{
            //    userDTO.UserType = UserType.Admin;
            //}

            //loading = true;
            //var responseHttp = await Repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
            //loading = false;
            //if (responseHttp.Error)
            //{
            //    var message = await responseHttp.GetErrorMessageAsync();
            //    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            //    return;
            //}
            //await SweetAlertService.FireAsync("Confirmación", "Su cuenta ha sido creada con exito. Se te ha enviado un correo electrónico con las instrucciones para activar tu usuario.", SweetAlertIcon.Info);
            //NavigationManager.NavigateTo("/");
        }

        private void SuplierChanged(Supplier supplier)
        {
            selectedSupplier = supplier;
            temporalPurchaseDTO.SupplierId = supplier.Id;
        }

        private void ProductChanged(Product product)
        {
            selectedProduct = product;
            temporalPurchaseDTO.SupplierId = product.Id;
        }

        private async Task LoadSuppliersAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Supplier>>($"/api/suppliers/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
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
    }
}