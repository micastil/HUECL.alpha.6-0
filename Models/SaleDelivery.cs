using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace HUECL.alpha._6_0.Models
{
    public class SaleDelivery
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Numero de Despacho")]
        [Display(Name = "Numero Despacho")]
        public string Number { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar una Fecha de Despacho")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Despacho")]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Modificacion")]
        public DateTime ModificationDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Total Despacho")]
        public decimal TotalDelivery { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Total Neto")]
        public decimal TotalNet { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Total Impuesto")]
        public decimal TotalTax { get; set; }

        [Display(Name = "Comentario")]
        public string? Comment { get; set; }

        [Required]
        public Active Active { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public DeliveryState DeliveryState { get; set; }

        [ForeignKey("FK_SaleDelivery_Sale_SaleId")]
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public ICollection<SaleDeliveryItem> SaleDeliveryItems { get; set; } = null!;
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = null!;
    }
}
