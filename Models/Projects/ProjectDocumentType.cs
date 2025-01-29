using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class ProjectDocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;

        public ICollection<ProjectDocument> Documents { get; set; } = [];
    }
}
