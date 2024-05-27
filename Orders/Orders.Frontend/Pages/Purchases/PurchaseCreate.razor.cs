using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;

namespace Orders.Frontend.Pages.Purchases
{
    public partial class PurchaseCreate
    {
        private PurchaseDTO purchaseDTO = new();
        private List<Supplier>? suppliers;
        private Supplier selectedSupplier = new();

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSuppliersAsync();
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

        private void CityChanged(Supplier supplier)
        {
            selectedSupplier = supplier;
            purchaseDTO.SupplierId = supplier.Id;
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
    }
}