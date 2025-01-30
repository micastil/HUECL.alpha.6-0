using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class ProjectSector
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a Name")]
        [Display(Name = "Project Sector")]
        public string Name { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public ICollection<Project> Projects { get; set; } = [];
    }
}
