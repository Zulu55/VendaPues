using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Shared.Entities
{
    public class InventoryDetail
    {
        public int Id { get; set; }

        public Inventory? Inventory { get; set; }

        public int InventoryId { get; set; }

        public Product? Product { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Costo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Cost { get; set; }

        [Display(Name = "Conteo 1")]
        public float Count1 { get; set; }

        [Display(Name = "Conteo 2")]
        public float Count2 { get; set; }

        [Display(Name = "Conteo 3")]
        public float Count3 { get; set; }
    }
}
