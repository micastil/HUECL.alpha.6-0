using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models
{
    public class SaleDeliveryItemViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un nombre de Producto")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ingresar valores mayores que Cero")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Required]
        public int SaleDeliveryId { get; set; }

    }
}
