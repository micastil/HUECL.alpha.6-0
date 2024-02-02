using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    [Index(nameof(InternalCode), IsUnique = true)]
    public class Product
    {
        /* Properties */

        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un nombre de Producto")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un Código de Producto")]
        [Display(Name = "Codigo Interno")]
        public int InternalCode { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Costo Unitario")]
        public decimal UnitaryCost { get; set; }

        [Required]
        public int Active { get; set; }

        [Display(Name = "Codigo Proveedor")]
        public string? ManufacturerCode { get; set; }

        public string? Description { get; set; }

        /* References */

        [ForeignKey("Provider")]
        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "Debe seleccionar Proveedor")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; } = null!;

        [ForeignKey("SubCategory")]
        [Display(Name = "Sub Categoria")]
        [Required(ErrorMessage = "Debe seleccionar Sub Categoria")]
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; } = null!;

        [ForeignKey("Unit")]
        [Display(Name = "Unidad")]
        [Required(ErrorMessage = "Debe seleccionar Unidad")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }= null!;

        public ICollection<SaleItem>? SaleItems { get; set; }

    }
}
