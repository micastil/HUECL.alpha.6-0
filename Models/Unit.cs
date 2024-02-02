using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HUECL.alpha._6_0.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Nombre")]
        [Display(Name = "Nombre Unidad")]
        public string Name { get; set; } = null!;
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Codigo de Unidad")]
        [Display(Name = "Unidad")]
        public string Code { get; set; } = null!;
        
        [Required]
        public int Active { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
