using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un Numero de Orden")]
        [Display(Name = "Numero OC")]
        public string Number { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar una Fecha de Orden")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha OC")]
        public DateTime Date { get; set; }

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
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public SaleState SaleState { get; set; }

        [Required]
        public Active Active { get; set; }

        [Display(Name = "Comentario")]
        public string? Comment { get; set; }

        [ForeignKey("FK_Sale_Customer_CustomerId")]
        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "Debe seleccionar Cliente")]
        public int CustomerId { get; set; }
        [Display(Name = "Cliente")]
        public Customer Customer { get; set; } = null!;

        [ForeignKey("FK_Sale_Currency_CurrencyId")]
        [Display(Name = "Moneda")]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public ICollection<SaleItem> SaleItems { get; set; } = null!;
        public ICollection<SaleDelivery> SaleDeliveries { get; set; } = null!;

    }
}
