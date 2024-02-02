using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HUECL.alpha._6_0.Models
{
    public class SaleDeliveryItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la Cantidad del Despacho")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Sub Total")]
        public Decimal SubTotal { get; set; }

        [Required]
        public Active Active { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Creacion")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Modificacion")]
        public DateTime ModificationDate { get; set; }

        [Required]
        [ForeignKey("FK_SaleDeliveryItem_SaleDelivery_Id")]
        public int SaleDeliveryId { get; set; }
        public SaleDelivery SaleDelivery { get; set; } = null!;

        [Required]
        [ForeignKey("FK_SaleDeliveryItem_SaleItem_SaleItemId")]
        public int SaleItemId { get; set; }
        public SaleItem SaleItem { get; set; } = null!;
    }
}
