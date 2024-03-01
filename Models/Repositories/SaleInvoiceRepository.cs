using HUECL.alpha._6_0.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models
{
    public class SaleInvoiceRepository : ISaleInvoiceRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SaleRepository> _logger;
        private readonly ISaleDeliveryRepository _saleDeliveryRepository;

        public SaleInvoiceRepository(AppDbContext appDbContext, ILogger<SaleRepository> logger, ISaleDeliveryRepository saleDeliveryRepository)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _saleDeliveryRepository = saleDeliveryRepository;
        }

        public async Task<int> AddSaleInvoice(SaleInvoice entity)
        {
            try
            {

                var _delivery = await _saleDeliveryRepository.GetSaleDeliveryById(entity.SaleDeliveryId);

                if (_delivery != null) 
                {
                    _delivery.DeliveryState = DeliveryState.WithInvoice;
                    _delivery.ModificationDate = DateTime.Now;

                    entity.Active = Active.Active;
                    entity.ModificationDate = DateTime.Now;
                    entity.InvoiceState = InvoiceState.NoPayment;

                    _appDbContext.SaleInvoices.Add(entity);

                }

                return await _appDbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public Task<SaleInvoicePayment?> GetAllSaleInvoicePayment(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<SaleInvoice?> GetSaleInvoiceById(int Id)
        {
            try
            {
                return await _appDbContext.
                    SaleInvoices.
                    Include(i => i.SaleInvoicePayments).
                    FirstOrDefaultAsync(t => t.Id == Id && t.Active == Active.Active);

            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<bool> SaleInvoiceExistis(int Id)
        {
            try
            {
                return await _appDbContext.SaleInvoices.AnyAsync(t => t.Id == Id && t.Active == Active.Active);
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new SaleRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }
    }
}
