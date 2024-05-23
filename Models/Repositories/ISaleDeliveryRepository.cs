using HUECL.alpha._6_0.Models.ViewModels;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public interface ISaleDeliveryRepository
    {
        Task<bool> SaleDeliveryItemExists(int Id);
        Task<bool> SaleDeliveryExists(int Id);
        Task<int> DeleteSaleDeliveryItemById(int Id);
        Task<int> DeleteSaleDelivery(SaleDelivery saleDelivery);
        Task<SaleDelivery?> GetSaleDeliveryById(int Id);
        Task<IEnumerable<SaleDelivery>> GetAllSaleDeliveriesBySaleId(int SaleId);
        Task<DataTablesViewModel<SaleDeliveryViewModel>> GetDataTablesSaleDelivery(
            string? draw,
            int skip,
            int pageSize,
            string? searchValue,
            int sortColumnIndex,
            string? sortColumnName,
            string? sortDirection,
            int selectedYear
        );
    }
}
