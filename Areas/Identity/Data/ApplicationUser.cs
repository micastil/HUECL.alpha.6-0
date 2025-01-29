using DocumentFormat.OpenXml.Wordprocessing;
using HUECL.alpha._6_0.Models.Projects;
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

        public ICollection<ProjectUser> ProjectUsers { get; set; } = [];
        public ICollection<Interaction> CreatedInteractions { get; set; } = [];
        public ICollection<Reminder> AssignedReminders { get; set; } = [];
        public ICollection<Project> OwnedProjects { get; set; } = [];
    }
}
