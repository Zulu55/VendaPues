using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.News
{
    public partial class NewsForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [Parameter, EditorRequired] public NewsArticle NewsArticle { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter] public bool IsEdit { get; set; } = false;

        public bool FormPostedSuccessfully { get; set; } = false;
        private string titleLabel => IsEdit ? "Editar Noticia" : "Crear Noticia";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            editContext = new(NewsArticle);
        }

        private void ImageSelected(string imagenBase64)
        {
            NewsArticle.ImageUrl = imagenBase64;
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            await OnValidSubmit.InvokeAsync();
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();

            if (!formWasEdited)
            {
                return;
            }

            if (FormPostedSuccessfully)
            {
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true
            });

            var confirm = !string.IsNullOrEmpty(result.Value);

            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}