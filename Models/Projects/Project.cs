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



        [Required]
        public bool Active { get; set; } = true;

        [Required]
        [Display(Name = "Creation Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Project Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "JHG Id")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Update")]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdate { get; set; }

        [Required(ErrorMessage = "You must enter a Project Name")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must enter a Priority")]
        [Display(Name = "Project Priority")]
        public int Priority { get; set; }

        [Required(ErrorMessage = "You must enter a Project Probability")]
        [Display(Name = "Project Probability")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Probaility { get; set; }

        [Required(ErrorMessage = "You must enter a Project Total")]
        [Display(Name = "Project Total")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Total { get; set; }


        [Required]
        [Display(Name="Project Ownner")]
        public string OwnerId { get; set; } = string.Empty;
        [Required]
        public ApplicationUser Owner { get; set; } = null!;

        [Required(ErrorMessage = "You must enter a Customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "You must enter a Project State")]
        [Display(Name = "Project State")]
        public int ProjectStatusId { get; set; }
        public ProjectStatus? ProjectStatus { get; set; }

        [Required(ErrorMessage = "You must enter a Project Sector")]
        [Display(Name = "Project Sector")]
        public int ProjectSectorId { get; set; }
        public ProjectSector? ProjectSector { get; set; }

        [Required(ErrorMessage = "You must enter a Project Currency")]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }


        public ICollection<Interaction> Interactions { get; set; } = [];
        public ICollection<ProjectDocument> Documents { get; set; } = [];
        public ICollection<Reminder> Reminders { get; set; } = [];
        public ICollection<ProjectUser> ProjectUsers { get; set; } = [];
    }
}
