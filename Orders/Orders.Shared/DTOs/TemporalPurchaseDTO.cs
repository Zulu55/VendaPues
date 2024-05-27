namespace Orders.Shared.DTOs
{
    public class TemporalPurchaseDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public float Quantity { get; set; } = 1;

        public decimal Cost { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}