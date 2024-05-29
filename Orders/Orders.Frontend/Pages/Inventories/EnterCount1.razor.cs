using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Frontend.Pages.Inventories
{
    public partial class EnterCount1
    {
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/InventoryDetails";
        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        private MudTable<InventoryDetail> table = new();
        public List<InventoryDetail>? InventoryDetails { get; set; }

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
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}&id={Id}";
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

        private async Task<TableData<InventoryDetail>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}&id={Id}";
            var responseHttp = await Repository.GetAsync<List<InventoryDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            InventoryDetails = responseHttp.Response;
            return new TableData<InventoryDetail>
            {
                Items = InventoryDetails,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private void SaveCount()
        {
            var isValid = ValidateCount();
            if (!isValid.WasSuccess)
            {
                ShowToast("Error", SweetAlertIcon.Error, isValid.Message!);
                return;
            }

            foreach (var inventoryDetail in InventoryDetails!)
            {
                _ = SaveInventoryDetailAsync(inventoryDetail);
            }
            ShowToast("Ok", SweetAlertIcon.Success, "Cambios guardados con exito.");
        }

        private ActionResponse<bool> ValidateCount()
        {
            foreach (var inventoryDetail in InventoryDetails!)
            {
                if (inventoryDetail.Count1 < 0 || inventoryDetail.Cost < 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo: {inventoryDetail.Cost} y conteo: {inventoryDetail.Count1}. No son permitidos valores negativos."
                    };
                }

                if (inventoryDetail.Count1 != 0 && inventoryDetail.Cost <= 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo 0 y una cantidad ingresada el el conteo de: {inventoryDetail.Count1}, debe ingresar un costo si ingresa conteo."
                    };
                }
            }

            return new ActionResponse<bool> {  WasSuccess = true };
        }

        private async Task SaveInventoryDetailAsync(InventoryDetail inventoryDetail)
        {
            var responseHttp = await Repository.PutAsync(baseUrl, inventoryDetail);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
                return;
            }
        }

        private async Task FinishCountAsync()
        {

        }


        private void ShowToast(string title, SweetAlertIcon iconMessage, string message)
        {
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            _ = toast.FireAsync(title, message, iconMessage);
        }
    }
}