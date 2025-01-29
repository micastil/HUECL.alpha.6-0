using HUECL.alpha._6_0.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class ProjectUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public bool CanAddInteractions { get; set; } = false;
    }
}
