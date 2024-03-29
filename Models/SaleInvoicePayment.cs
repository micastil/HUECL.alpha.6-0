﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class SaleInvoicePayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Active Active { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "Fecha Pago")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Debe ingrear un Total")]
        [Column(TypeName = "decimal(18,4)")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ingresar valores mayores que Cero")]
        [Display(Name = "Total Pago")]
        [DataType(DataType.Currency)]
        public Decimal TotalPayment { get; set; }

        [Display(Name = "Comentario")]
        public string? Comment { get; set; } = string.Empty;

        [ForeignKey("FK_SaleInvoicePayment_SaleInvoice_Id")]
        public int SaleInvoiceId { get; set; }
        public SaleInvoice SaleInvoice { get; set; } = null!;
    }
}
