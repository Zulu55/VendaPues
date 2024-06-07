using System.Net;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Pages.Cities;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.States
{
    [Authorize(Roles = "Admin")]
    public partial class StateDetails
    {
        private State? state;
        private List<City>? cities;

        private MudTable<City> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/cities";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Parameter] public int StateId { get; set; }

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadStateAsync()
        {
            var responseHttp = await Repository.GetAsync<State>($"/api/states/{StateId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/countries");
                    return false;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            state = responseHttp.Response;
            return true;
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            if (state is null)
            {
                var ok = await LoadStateAsync();
                if (!ok)
                {
                    NoState();
                    return false;
                }
            }

            var url = $"{baseUrl}/recordsnumber?id={StateId}&page=1&recordsnumber={int.MaxValue}";

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

        private async Task<TableData<City>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?id={StateId}&page={page}&recordsnumber={pageSize}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<City>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = message,
                    Icon = SweetAlertIcon.Error
                });
                return new TableData<City> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<City> { Items = [], TotalItems = 0 };
            }
            return new TableData<City>
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

        private void ReturnAction()
        {
            NavigationManager.NavigateTo($"/countries/details/{state?.CountryId}");
        }

        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true };
            IDialogReference? dialog;
            if (isEdit)
            {
                var parameters = new DialogParameters
                {
                    { "Id", id }
                };
                dialog = DialogService.Show<CityEdit>("Editar Ciudad", parameters, options);
            }
            else
            {
                var parameters = new DialogParameters
                {
                    { "StateId", StateId }
                };
                dialog = DialogService.Show<CityCreate>("Crear Ciudad", parameters, options);
            }

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }

        private void ResidentialUnitsAction(City city)
        {
            NavigationManager.NavigateTo($"/cities/details/{city.Id}");
        }

        private void NoState()
        {
            NavigationManager.NavigateTo("/countries");
        }

        private async Task DeleteAsync(City city)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Estás seguro de que quieres eliminar la ciudad {city.Name}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<City>($"api/cities/{city.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = message,
                        Icon = SweetAlertIcon.Error
                    });
                }
                return;
            }
            await LoadAsync();
            await table.ReloadServerData();
            ShowToast("Ok", SweetAlertIcon.Success, "Ciudad eliminada.");
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