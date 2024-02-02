using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Sub Categoria")]
        public string Name { get; set; } = null!;
        
        [Required]
        public string Code { get; set; } = null!;
        
        [Required]
        public int Active { get; set; }

        /* References */

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Product>? Products { get; set; }
    }
}
