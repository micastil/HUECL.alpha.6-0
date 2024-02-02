using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresa un Nombre de Categoria")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar un Codigo de Categoria")]
        public string Code { get; set; } = null!;
        
        [Required]
        public int Active { get; set; }

        /* References */
       
        public ICollection<SubCategory>? SubCategories { get; set; }
    }
}
