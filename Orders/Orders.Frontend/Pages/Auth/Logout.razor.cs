using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Services;

namespace Orders.Frontend.Pages.Auth
{
    public partial class Logout
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task LogoutActionAsync()
        {
            await LoginService.LogoutAsync();
            CancelAction();
        }

        private void CancelAction()
        {
            MudDialog.Cancel();
        }
    }
}