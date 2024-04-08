using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Debe ingresar Nombre")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar Apellido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;
    }
}
