using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
        [CascadingParameter] private BlazoredModalInstance BlazoredModal { get; set; } = default!;

        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/news", newsArticle);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await BlazoredModal.CloseAsync(ModalResult.Ok());
            Return();
            ShowToast("Ok", SweetAlertIcon.Success, "Registro creado con éxito.");
        }

        private void Return()
        {
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