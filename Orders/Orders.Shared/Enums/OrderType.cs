using System.ComponentModel;

namespace Orders.Shared.Enums
{
    public enum OrderType
    {
        [Description("Pago contra entrega")]
        PaymentAgainstDelivery,

        [Description("Pago en línea")]
        PayOnLine
    }
}