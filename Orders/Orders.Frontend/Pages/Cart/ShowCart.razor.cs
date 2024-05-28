using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class ShowCart
    {
        public List<TemporalOrder>? TemporalOrders { get; set; }
        private MudTable<TemporalOrder> table = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalOrders";

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        public OrderDTO OrderDTO { get; set; } = new();

        private async Task<TableData<TemporalOrder>> LoadListAsync(TableState state)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await Repository.GetAsync<List<TemporalOrder>>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }

            sumQuantity = responseHttp.Response.Sum(x => x.Quantity);
            sumValue = responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);

            return new TableData<TemporalOrder>
            {
                Items = responseHttp.Response,
                TotalItems = responseHttp.Response.Count
            };
        }

        private async Task ConfirmOrderAsync()
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres confirmar el pedido?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var httpActionResponse = await Repository.PostAsync("/api/orders", OrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            NavigationManager.NavigateTo("/Cart/OrderConfirmed");
        }

        private async Task DeleteAsync(int temporalOrderId)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Esta seguro que quieres borrar el registro?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);

            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<TemporalOrder>($"api/temporalOrders/{temporalOrderId}");

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }

                var mensajeError = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                return;
            }

            await table.ReloadServerData();
            ShowToast("Ok", SweetAlertIcon.Success, "Producto eliminado del carro de compras.");
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
