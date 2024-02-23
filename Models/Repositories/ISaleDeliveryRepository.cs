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
    }
}
