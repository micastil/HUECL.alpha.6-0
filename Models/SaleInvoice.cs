using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class SaleInvoice
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Numero de Factura")]
        [StringLength(50)]
        [Display(Name = "Numero Factura")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una Fecha de Factura")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Factura")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Modificacion")]
        public DateTime ModificationDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Pago")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        public Active Active { get; set; }

        [Display(Name = "Comentario")]
        public string? Comment { get; set; } = String.Empty;

        [ForeignKey("FK_SaleInvoice_SaleDelivery_Id")]
        public int SaleDeliveryId { get; set; }
        public SaleDelivery SaleDelivery { get; set; } = null!;

        public ICollection<SaleInvoicePayment>? SaleInvoicePayments { get; set; }
    }
}
