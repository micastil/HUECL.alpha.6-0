using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public class SaleDeliveryRepository : ISaleDeliveryRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SaleRepository> _logger;

        public SaleDeliveryRepository(AppDbContext appDbContext, ILogger<SaleRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<int> DeleteSaleDelivery(SaleDelivery saleDelivery)
        {
            try
            {
                _appDbContext.SaleDeliveries.Remove(saleDelivery);
                int _result = await _appDbContext.SaveChangesAsync();

                if(_result > 0 ) 
                {
                    int _deliveriesCount = await _appDbContext
                        .SaleDeliveries
                        .Where(a => a.SaleId ==  saleDelivery.SaleId && a.Active == Active.Active)
                        .CountAsync();

                    //TODO: Se debe modificar el SaleState en base a si se han despacho todos
                    //      los item que corresponden a la Sale.
                    if (_deliveriesCount == 0)
                    {
                        saleDelivery.Sale.SaleState = SaleState.NoDelivery;
                    }

                    if(_deliveriesCount > 0 && saleDelivery.SaleDeliveryItems.Count > 0) 
                    {
                        saleDelivery.Sale.SaleState = SaleState.PartialDelivery;
                    } 

                    await _appDbContext.SaveChangesAsync();
                }

                return _result;
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

        public async Task<int> DeleteSaleDeliveryItemById(int Id)
        {
            try
            {
                int _deleteResult = 0;

                if (await SaleDeliveryItemExists(Id)) 
                {
                    var _deliveryItemtoDelete = await _appDbContext.
                        SaleDeliveryItems.
                        Include(s => s.SaleDelivery).
                        ThenInclude(t => t.Sale).
                        FirstOrDefaultAsync(t => t.Id == Id);

                    if(_deliveryItemtoDelete != null) 
                    {
                        _deliveryItemtoDelete.SaleDelivery.TotalNet -= _deliveryItemtoDelete.SubTotal;
                        _deliveryItemtoDelete.SaleDelivery.TotalTax -= (int)(_deliveryItemtoDelete.SubTotal * (Decimal)IVARate.CL);
                        _deliveryItemtoDelete.SaleDelivery.TotalDelivery = (int)(_deliveryItemtoDelete.SaleDelivery.TotalNet + _deliveryItemtoDelete.SaleDelivery.TotalTax);

                        if (_deliveryItemtoDelete.SaleDelivery.TotalNet == 0) 
                        {
                            _deliveryItemtoDelete.SaleDelivery.DeliveryState = DeliveryState.Empty;
                        }

                        _deliveryItemtoDelete.SaleDelivery.Sale.SaleState = SaleState.PartialDelivery;

                        _appDbContext.SaleDeliveryItems.Remove(_deliveryItemtoDelete);

                        _deleteResult = await _appDbContext.SaveChangesAsync();
                    }
                }

                return _deleteResult;
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

        public async Task<SaleDelivery?> GetSaleDeliveryById(int Id)
        {
            try 
            {
                return await _appDbContext
                    .SaleDeliveries
                    .Include(s => s.Sale)
                    .Include(i => i.SaleDeliveryItems)
                    .Include(p => p.SaleInvoice)
                    .FirstOrDefaultAsync(t => t.Id == Id && t.Active == Active.Active);
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

        public async Task<bool> SaleDeliveryExists(int Id)
        {
            try 
            {
                return await _appDbContext.SaleDeliveries.AnyAsync(e => e.Id == Id && e.Active == Active.Active);
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

        public async Task<bool> SaleDeliveryItemExists(int Id)
        {
            try 
            {
                return await _appDbContext.SaleDeliveryItems.AnyAsync(e => e.Id == Id);
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

        public async Task<IEnumerable<SaleDelivery>> GetAllSaleDeliveriesBySaleId(int SaleId)
        {
            try
            {
                return await _appDbContext.
                    SaleDeliveries.Where(t => t.SaleId == SaleId && t.Active == Active.Active)
                    .Include(j => j.SaleInvoice)
                    .Include(s => s.Sale)
                        .ThenInclude(c => c.Customer)
                    .Include(i => i.SaleDeliveryItems)
                        .ThenInclude(i => i.SaleItem)
                        .ThenInclude(p => p.Product)
                    .ToListAsync();
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
