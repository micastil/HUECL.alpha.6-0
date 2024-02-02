namespace HUECL.alpha._6_0.Models
{
    public interface IProductRepository
    {
        Task<DataTablesViewModel<ProductViewModel>> GetDataTablesProduct(
            string? draw, 
            int skip, 
            int pageSize,
            string? searchValue,
            int sortColumnIndex, 
            string? sortColumnName, 
            string? sortDirection);
        Task<IEnumerable<Product>> GetPageSearchProducts(int page, int pageSize, string searchTerm);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int ProductId);
        Task<Product> GetProductByCode(string internalcode);

        Task<int> AddProduct(Product newProduct);

        Task<int> UpdateProduct(int id, string comment);
        Task<bool> IsValid(int id);

        Task<int> TotalProducts();
    }
}
