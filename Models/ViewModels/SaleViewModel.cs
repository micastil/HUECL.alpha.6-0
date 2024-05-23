namespace HUECL.alpha._6_0.Models
{
    public class SaleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Customer { get; set; }
        public string? Number { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string? State { get; set; }
    }
}
