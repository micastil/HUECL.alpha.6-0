namespace HUECL.alpha._6_0.Models
{
    public class DataTablesViewModel<T>
    {
        public IEnumerable<T> Data { get; set; } = null!;
        public string? draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
    }
}
