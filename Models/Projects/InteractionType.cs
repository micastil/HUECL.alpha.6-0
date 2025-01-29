using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class InteractionType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Interaction Name")]
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;

        public ICollection<Interaction> Interactions { get; set; } = [];
    }
}
