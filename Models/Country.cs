using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Nombre de Pais")]
        [Display(Name = "Pais")]
        public string Name { get; set; } = null!;

        [Display(Name = "Codigo Pais")]
        public string? Code { get; set; }

        public ICollection<Sale>? Sale { get; set; }
    }
}
