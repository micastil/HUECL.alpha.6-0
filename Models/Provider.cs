using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HUECL.alpha._6_0.Models
{
    public class Provider
    {
        [Key]
        public int Id { get; set; }
        
        [Required (ErrorMessage = "Debe ingresar un Nombre de Proveedor")]
        [Display(Name = "Proveedor")]
        public string Name { get; set; } = null!;
        
        [Required]
        public int Active { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
