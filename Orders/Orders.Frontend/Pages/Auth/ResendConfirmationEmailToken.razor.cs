using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;

namespace Orders.Frontend.Pages.Auth
{
    public partial class ResendConfirmationEmailToken
    {
        private EmailDTO emailDTO = new();
        private bool loading;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        private async Task ResendConfirmationEmailTokenAsync()
        {
            loading = true;
            var responseHttp = await Repository.PostAsync("/api/accounts/ResedToken", emailDTO);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            Snackbar.Add("Se te ha enviado un correo electrónico con las instrucciones para activar tu usuario.", Severity.Success);
            NavigationManager.NavigateTo("/");
        }
    }
}