using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class ProjectStatus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a Name")]
        [Display(Name = "Project State Name")]
        public string Name { get; set; } = String.Empty;

        [Required]
        public bool Active { get; set; } = true;

        public ICollection<Project> Projects { get; set; } = [];
    }
}
