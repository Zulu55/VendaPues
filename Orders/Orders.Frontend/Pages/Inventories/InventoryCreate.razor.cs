using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Inventories
{
    public partial class InventoryCreate
    {
        private Inventory inventory = new() { Date = DateTime.Now };

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        private async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            inventory.Date = (DateTime)date;
        }

        private async Task SaveInventoryAsync()
        {
            if (string.IsNullOrEmpty(inventory.Name))
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes ingresar un nombre al inventario.");
                return;
            }

            if (string.IsNullOrEmpty(inventory.Description))
            {
                ShowToast("Error", SweetAlertIcon.Error, "Debes ingresar una descripción al inventario.");
                return;
            }
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres crear este nuevo inventario?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.PostAsync<Inventory>("/api/inventories", inventory);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            ShowToast("Ok", SweetAlertIcon.Success, "Inventario creado.");
            NavigationManager.NavigateTo("/inventories");
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