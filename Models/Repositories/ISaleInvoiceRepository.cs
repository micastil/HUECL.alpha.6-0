namespace HUECL.alpha._6_0.Models
{
    public interface ISaleInvoiceRepository
    {
        Task<int> AddSaleInvoice(SaleInvoice entity);
        Task<bool> SaleInvoiceExistis(int Id);
        Task<SaleInvoice?> GetSaleInvoiceById(int Id);
    }
}
