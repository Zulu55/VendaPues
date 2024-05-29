using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Frontend.Pages.Inventories
{
    public partial class EnterCount2
    {
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/InventoryDetails";
        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
        private bool enableModifyCost = false;

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

        private void OnEnableModifyCostChanged(bool value)
        {
            enableModifyCost = value;
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsNumberCount2?page=1&recordsnumber={int.MaxValue}&id={Id}";
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

        private async Task<TableData<InventoryDetail>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}/Count2?page={page}&recordsnumber={pageSize}&id={Id}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

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
            if (InventoryDetails == null) return;

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
                if (inventoryDetail.Count2 < 0 || inventoryDetail.Cost < 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo: {inventoryDetail.Cost} y conteo: {inventoryDetail.Count2}. No son permitidos valores negativos."
                    };
                }

                if (inventoryDetail.Count2 != 0 && inventoryDetail.Cost <= 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo 0 y una cantidad ingresada el el conteo de: {inventoryDetail.Count2}, debe ingresar un costo si ingresa conteo."
                    };
                }
            }

            return new ActionResponse<bool> { WasSuccess = true };
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
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres finalizar el conteo #2?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var count0 = InventoryDetails!.Count(x => x.Count2 == 0);
            if (count0 / InventoryDetails!.Count > 0.5)
            {
                result = await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Confirmación",
                    Text = "Hay una gran cantidad de productos con conteo en cero, ¿Estas seguro de cerrar este primer conteo?",
                    Icon = SweetAlertIcon.Question,
                    ShowCancelButton = true
                });

                confirm = string.IsNullOrEmpty(result.Value);
                if (confirm)
                {
                    return;
                }
            }

            var responseHttp = await Repository.GetAsync($"/api/inventories/finishCount1/{Id}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }

            ShowToast("Ok", SweetAlertIcon.Success, "Conteo #1 cerrado, puede proceder al conteo #2.");
            NavigationManager.NavigateTo("/inventories");
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