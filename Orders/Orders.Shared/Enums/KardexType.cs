﻿using System.ComponentModel;

namespace Orders.Shared.Enums
{
    public enum KardexType
    {
        [Description("Compra")]
        Purchase,

        [Description("Pedido")]
        Order
    }
}