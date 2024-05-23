namespace HUECL.alpha._6_0.Models.ViewModels
{
    public class SaleDeliveryViewModel
    {
        public string Id { get; set; } =string.Empty;
        public string Number { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Customer { get; set; } = string.Empty;
        public int Total { get; set; }
        public string State { get; set; } = string.Empty;
        public string SaleNumber { get; set; } = string.Empty;
        public string SaleId { get; set; } = string.Empty;
    }
}
