using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar un Nombre de Moneda")]
        [Display(Name = "Moneda")]
        public string Name { get; set; } = null!;

        [Display(Name = "Codigo Moneda")]
        public string? Code { get; set; }

        public ICollection<Sale>? Sale { get; set; }
    }
}
