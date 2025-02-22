using HUECL.alpha._6_0.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class Interaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        [Display(Name = "Interaction Type")]
        public int InteractionTypeId { get; set; }
        public InteractionType InteractionType { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Created by")]
        public string CreatedByUserId { get; set; } = String.Empty;
        public ApplicationUser CreatedByUser { get; set; } = null!;
    }
}
