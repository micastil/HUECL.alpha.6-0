using HUECL.alpha._6_0.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a Project Name")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Project Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name="Project Ownner")]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        [Required(ErrorMessage = "You must enter a Customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "You must enter a Project State")]
        [Display(Name = "Project State")]
        public int ProjectStatusId { get; set; }
        public ProjectStatus? ProjectStatus { get; set; }

        public ICollection<Interaction> Interactions { get; set; } = [];
        public ICollection<ProjectDocument> Documents { get; set; } = [];
        public ICollection<Reminder> Reminders { get; set; } = [];
        public ICollection<ProjectUser> ProjectUsers { get; set; } = [];
    }
}
