using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Products
{
    public partial class ProductDetails
    {
        private List<string>? categories;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private bool isAuthenticated;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int ProductId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; } = null!;
        public TemporalOrderDTO TemporalOrderDTO { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await authenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
        }

        private async Task LoadProductAsync()
        {
            loading = true;
            var httpActionResponse = await repository.GetAsync<Product>($"/api/products/{ProductId}");

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            product = httpActionResponse.Response!;
            categories = product.ProductCategories!.Select(x => x.Category!.Name).ToList();
            images = product.ProductImages!.Select(x => x.Image).ToList();
            loading = false;
        }

        public async Task AddToCartAsync()
        {
            if (!isAuthenticated)
            {
                NavigationManager.NavigateTo("/Login");
                ShowToast("Error", SweetAlertIcon.Error, "Debes haber iniciado sesión para poder agregar productos al carro de compras.");
                return;
            }

            TemporalOrderDTO.ProductId = ProductId;

            var httpActionResponse = await repository.PostAsync("/api/temporalOrders/full", TemporalOrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            var toast2 = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            NavigationManager.NavigateTo("/");
            ShowToast("Ok", SweetAlertIcon.Success, "Producto agregado al carro de compras.");
        }

        private void ShowToast(string title, SweetAlertIcon iconMessage, string message)
        {
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            _ = toast.FireAsync(title, message, iconMessage);
        }
    }
}