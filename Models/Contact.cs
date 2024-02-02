using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Nombre")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "Posicion")]
        public string? Position { get; set; }

        [Display(Name = "Area")]
        public string? Area { get; set; }

        [Display(Name = "Telefono")]
        public string? Phone { get; set; }

        [Display(Name = "Movil")]
        public string? Mobile { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Direccion")]
        public string? Address { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }

        public Active Active { get; set; }

        public ICollection<Customer>? Customers { get; set; }
    }
}
