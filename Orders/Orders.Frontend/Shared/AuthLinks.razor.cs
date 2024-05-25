using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Blazored.Modal.Services;
using Orders.Frontend.Pages.Auth;

namespace Orders.Frontend.Shared
{
    public partial class AuthLinks
    {
        [CascadingParameter] IModalService Modal { get; set; } = default!;

        private void ShowModal()
        {
            Modal.Show<Login>();
        }
    }
}
