using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Inventories
{
    [Authorize(Roles = "Admin")]
    public partial class InventoriesIndex
    {
        public List<Inventory>? Inventories { get; set; }

        private MudTable<Inventory> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/inventories";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}";
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

        private async Task<TableData<Inventory>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}";
            var responseHttp = await Repository.GetAsync<List<Inventory>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<Inventory> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<Inventory> { Items = [], TotalItems = 0 };
            }
            return new TableData<Inventory>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task ShowCount1ModalAsync(Inventory inventory)
        {
            var modalReference = Modal.Show<EnterCount1>(string.Empty, new ModalParameters().Add("Id", inventory.Id));
            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
            await table.ReloadServerData();
        }

        private async Task ShowCount2ModalAsync(Inventory inventory)
        {
            if (!inventory.Count1Finish)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Primero debes completar el conteo #1");
                return;
            }
            var modalReference = Modal.Show<EnterCount2>(string.Empty, new ModalParameters().Add("Id", inventory.Id));
            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
            await table.ReloadServerData();
        }

        private async Task ShowCount3ModalAsync(Inventory inventory)
        {
            if (!inventory.Count2Finish || !inventory.Count2Finish)
            {
                ShowToast("Error", SweetAlertIcon.Error, "Primero debes completar el conteo #1 y #2");
                return;
            }
            var modalReference = Modal.Show<EnterCount3>(string.Empty, new ModalParameters().Add("Id", inventory.Id));
            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
            await table.ReloadServerData();
        }

        private async Task ShowCreateModalAsync()
        {
            var modalReference = Modal.Show<InventoryCreate>();
            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
            await table.ReloadServerData();
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