using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages
{
    public partial class NewsAndPromotions
    {
        private List<NewsArticle>? newsArticles;
        private const string baseUrl = "api/news";
        private bool loading = true;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            loading = true;
            var url = $"{baseUrl}?page=1&recordsnumber={int.MaxValue}&Id=1";
            var responseHttp = await Repository.GetAsync<List<NewsArticle>>(url);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
            }

            newsArticles = responseHttp.Response;
        }

        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }
        
        private void ViewDetails(int id)
        {
            // Aquí puedes implementar la navegación a una página de detalles o mostrar un diálogo con más información.
        }
    }
}