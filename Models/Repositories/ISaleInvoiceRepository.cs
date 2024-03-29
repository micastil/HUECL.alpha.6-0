﻿namespace HUECL.alpha._6_0.Models
{
    public interface ISaleInvoiceRepository
    {
        Task<int> AddSaleInvoice(SaleInvoice entity);
        Task<int> AddSaleInvoicePayment(SaleInvoicePayment saleInvoicePayment);
        Task<int> DeleteInvoicePaymet(int id);
        Task<IEnumerable<SaleInvoicePayment>?> GetAllSaleInvoicePayment(int Id);
        Task<SaleInvoice?> GetSaleInvoiceById(int Id);
        Task<SaleInvoicePayment?> GetSaleInvoicePaymentById(int Id);
        Task<bool> SaleInvoiceExistis(int Id);
    }
}
