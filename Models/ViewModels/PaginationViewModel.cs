namespace HUECL.alpha._6_0.Models
{
    public class PaginationViewModel<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
