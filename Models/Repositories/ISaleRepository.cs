namespace HUECL.alpha._6_0.Models
{
    public interface ISaleRepository
    {
        /**************************   SALES ********************************************************/
        Task<DataTablesViewModel<SaleViewModel>> GetDataTablesSale(
            string? draw,
            int skip,
            int pageSize,
            string? searchValue,
            int sortColumnIndex,
            string? sortColumnName,
            string? sortDirection);
        Task<IEnumerable<Sale>> GetAllSales();
        Task<Sale> GetSaleById(int id);
        Task<int> AddSale(Sale entity);

        /**************************   SALE_ITEM ********************************************************/
        Task<IEnumerable<SaleItem>> GetAllSaleItemsBySaleId(int SaleId);
        Task<SaleItem> GetSaleItemById(int id);
        Task<int> AddSaleItem(SaleItem entity);
        Task<int> DeleteSaleItem(int SaleItemId);
        Task<bool> SaleItemExist(int SaleItemId);
        Task<bool> SaleItemHasDelivery(int SaleItemId);

        /**************************   SALE_DELIVERY *******************************************************/
        Task<IEnumerable<SaleDelivery>> GetAllSaleDeliveriesBySaleId(int SaleId);
        Task<int> AddSaleDelivery(SaleDelivery entity);
        Task<SaleDelivery> GetSaleDeliveryById(int SaleDeliveryId);
        //Task<int> AddSaleDeliveryItems(ICollection<SaleDeliveryItem> _list);
        Task<int> AddSaleDeliveryItems(ICollection<SaleDeliveryItemViewModel> _list);
        Task<IEnumerable<SaleDeliveryItem>> GetAllSaleDeliveryItemsByDeliveryId(int SaleDeliveryId);
        Task<bool> CheckQuantityDeliveryItem(SaleDeliveryItem entity);
        Task<ICollection<SaleDeliveryItemViewModel>> GetItemsAvailableForDelivery(int SaleId, int SaleDeliveryId);
        Task<SaleDeliveryItem> GetSaleDeliveryItemById(int SaleDeliveryItemId);
        Task<int> DeleteSaleDeliveryItemById(int Id);

        /**************************   SALE_INVOICE *******************************************************/
        Task<int> AddSaleInvoice(SaleInvoice entity);
    }
}
