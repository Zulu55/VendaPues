using Microsoft.AspNetCore.Components;

namespace Orders.Frontend.Shared
{
    public partial class Loading
    {
        [Parameter] public string Label { get; set; } = "Por favor espera...";
    }
}
