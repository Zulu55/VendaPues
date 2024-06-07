using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.News
{
    [Authorize(Roles = "Admin")]
    public partial class NewsCreate
    {
        private NewsArticle newsArticle = new() { Active = true };
        private NewsForm? newsForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/news", newsArticle);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            newsForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/news");
            ShowToast("Ok", SweetAlertIcon.Success, "Registro creado con éxito.");
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            newsForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/news");
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