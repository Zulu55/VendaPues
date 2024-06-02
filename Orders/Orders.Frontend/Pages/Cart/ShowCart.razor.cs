using System.Net.Mail;
using System.Security.AccessControl;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

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
        private Bank selectedBank = new();
        private List<Bank>? banks;
        private string email = null!;
        private bool loading;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        public OrderDTO OrderDTO { get; set; } = new();
        private int selectedPaymentOption { get; set; }


        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadBanksAsync();
        }

        private async Task LoadBanksAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Bank>>($"/api/banks/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                ShowToast("Error", SweetAlertIcon.Error, message!);
                return;
            }
            banks = responseHttp.Response;
        }

        private void BankChanged(Bank supplier)
        {
            selectedBank = supplier;
        }

        private async Task<IEnumerable<Bank>> SearchBankAsync(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return banks!;
            }

            return banks!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

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
            if (selectedPaymentOption == 1)
            {
                if (selectedBank.Id == 0)
                {
                    ShowToast("Error", SweetAlertIcon.Error, "Debes seleccionar un banco.");
                    return;
                }

                if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
                {
                    ShowToast("Error", SweetAlertIcon.Error, "Debes ingresar un email válido.");
                    return;
                }
            }

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

            if (selectedPaymentOption == 1)
            {
                loading = true;
                var paymentDTO = new PaymentDTO
                {
                    BankId = selectedBank.Id,
                    Email = email,
                    Value = sumValue
                };
                var httpActionPaymenyResponse = await Repository.PostAsync<PaymentDTO, ActionResponse<string>>("/api/payments", paymentDTO);
                var response = httpActionPaymenyResponse.Response;
                loading = false;

                if (!response!.WasSuccess)
                {
                    await SweetAlertService.FireAsync("Error", response.Message, SweetAlertIcon.Error);
                    return;
                }

                OrderDTO.Email = email;
                OrderDTO.BankId = selectedBank.Id;
                OrderDTO.Value = sumValue;
                OrderDTO.Reference = response.Result!;
            }

            if (selectedPaymentOption == 0)
            {
                OrderDTO.OrderType = OrderType.PaymentAgainstDelivery;
                OrderDTO.Email = "none@none.com";
                OrderDTO.Reference = "NA";
            }
            else
            {
                OrderDTO.OrderType = OrderType.PayOnLine;
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
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
                ShowConfirmButton = true,
                Timer = 3000
            });
            _ = toast.FireAsync(title, message, iconMessage);
        }
    }
}
