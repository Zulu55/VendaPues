using System.ComponentModel.DataAnnotations;

namespace Orders.Shared.DTOs
{
    public class PurchaseDetailDTO
    {
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string? Remarks { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int ProductId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Quantity { get; set; }
    }
}