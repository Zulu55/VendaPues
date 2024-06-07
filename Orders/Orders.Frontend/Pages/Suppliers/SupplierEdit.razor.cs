using System.Net;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Suppliers
{
    [Authorize(Roles = "Admin")]
    public partial class SupplierEdit
    {
        private Supplier? supplier;
        private SupplierForm? supplierForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<Supplier>($"/api/suppliers/one/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/suppliers");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                supplier = responseHttp.Response;
            }
        }

        private async Task EditAsync()
        {
            var responseHttp = await Repository.PutAsync("/api/suppliers", supplier);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
            ShowToast("Ok", SweetAlertIcon.Success, "Cambios guardados con éxito.");
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
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