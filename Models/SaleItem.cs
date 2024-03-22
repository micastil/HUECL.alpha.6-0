using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HUECL.alpha._6_0.Models
{
    public class SaleItem
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Debe ingresar un codigo de cliente")]
        [Display(Name ="Codigo Cliente")]
        public string CustomerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar Fecha de Entrega")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        public DateTime RequestedDelivery { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Modificacion")]
        public DateTime ModificationDate { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Precio Unitario")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Precio Unitario")]
        public Decimal UnitaryPrice { get; set; }

        [Required(ErrorMessage = "Debe ingresar una cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ingresar valores mayores que Cero")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Sub Total")]
        public Decimal SubTotal { get; set; }

        [Required]
        public Active Active { get; set; }

        [ForeignKey("FK_SaleItem_Sale_SaleId")]
        [Display(Name = "Orden de Compra")]
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        [ForeignKey("FK_SaleItem_Sale_ProductId")]
        [Display(Name = "Producto")]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
