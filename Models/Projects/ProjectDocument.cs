using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class ProjectDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        public int DocumentTypeId { get; set; }
        public ProjectDocumentType? DocumentType { get; set; }

        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
