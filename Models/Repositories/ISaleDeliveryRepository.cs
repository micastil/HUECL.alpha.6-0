namespace HUECL.alpha._6_0.Models.Repositories
{
    public interface ISaleDeliveryRepository
    {
        Task<bool> SaleDeliveryItemExists(int Id);
        Task<int> DeleteSaleDeliveryItemById(int Id);
    }
}
