namespace HUECL.alpha._6_0.Models.ViewModels
{
    public class ProjectViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public string Owner { get; set; } = string.Empty;

    }
}
