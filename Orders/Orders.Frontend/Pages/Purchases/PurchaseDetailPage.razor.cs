using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Pages.Purchases
{
    [Authorize(Roles = "Admin")]
    public partial class PurchaseDetailPage
    {
        public List<PurchaseDetail>? PurchaseDetails { get; set; }
        public Purchase? Purchase { get; set; }

        private MudTable<PurchaseDetail> table = new();
        private readonly int[] pageSizeOptions = { 5, 10 };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/purchaseDetails";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public int PurchaseId { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecordsAsync();
            await LoadPurchaseAsync();
        }

        private async Task LoadPurchaseAsync()
        {
            loading = true;
            var url = $"api/purchases/{PurchaseId}";
            var responseHttp = await Repository.GetAsync<Purchase>(url);
            loading = false;

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/purchase");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                Purchase = responseHttp.Response;
            }
        }

        private async Task<bool> LoadTotalRecordsAsync()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}&id={PurchaseId}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }

        private async Task<TableData<PurchaseDetail>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}&id={PurchaseId}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<PurchaseDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<PurchaseDetail> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<PurchaseDetail> { Items = [], TotalItems = 0 };
            }
            return new TableData<PurchaseDetail>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }
    }
}