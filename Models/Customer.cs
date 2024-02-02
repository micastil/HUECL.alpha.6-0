using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Nombre de Proveedor")]
        [Display(Name = "Nombre Cliente")]
        public string Name { get; set; } = null!;

        [Display(Name = "RUT/RUC/TaxId")]
        public string? TaxId { get; set; }

        [Display(Name = "Direccion")]
        public string? Address { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }
        
        [Required]
        public Active Active { get; set; }

        [ForeignKey("FK_Customer_Country_CountryId")]
        [Display(Name = "Pais")]
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public ICollection<Sale>? Sales { get; set; }

    }
}
