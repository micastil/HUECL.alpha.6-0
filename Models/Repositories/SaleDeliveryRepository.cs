using DocumentFormat.OpenXml.Office2010.Excel;
using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models.CustomExceptions;
using HUECL.alpha._6_0.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public class SaleDeliveryRepository : ISaleDeliveryRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SaleRepository> _logger;
        private readonly ICustomDataProtector _customDataProtector;

        public SaleDeliveryRepository(AppDbContext appDbContext, ILogger<SaleRepository> logger, ICustomDataProtector customDataProtector)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _customDataProtector = customDataProtector;
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
                    .Include(s => s.Sale).ThenInclude(c => c.Customer)
                    .Include(i => i.SaleDeliveryItems).ThenInclude(a => a.SaleItem).ThenInclude(l => l.Product)
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

        public async Task<DataTablesViewModel<SaleDeliveryViewModel>> GetDataTablesSaleDelivery(string? draw, int skip, int pageSize, string? searchValue, int sortColumnIndex, string? sortColumnName, string? sortDirection, int selectedYear)
        {
            try
            {
                DataTablesViewModel<SaleDeliveryViewModel> dataTablesResult = new DataTablesViewModel<SaleDeliveryViewModel>();

                var query = _appDbContext.SaleDeliveries.Include(s => s.Sale).ThenInclude(c => c.Customer).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(p =>
                        (
                        p.Sale.Customer.Name.Contains(searchValue) ||
                        p.Number.Contains(searchValue) ||
                        p.Sale.Number.Contains(searchValue)
                        )
                    );
                }

                dataTablesResult.recordsFiltered = await query.CountAsync();

                switch (sortColumnName)
                {
                    case "customer":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Sale.Customer.Name)
                            : query.OrderByDescending(p => p.Sale.Customer.Name);
                        break;
                    case "number":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Number)
                            : query.OrderByDescending(p => p.Number);
                        break;
                    case "date":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.DeliveryDate)
                            : query.OrderByDescending(p => p.DeliveryDate);
                        break;
                    case "total":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.TotalNet)
                            : query.OrderByDescending(p => p.TotalNet);
                        break;
                }

                dataTablesResult.Data = await query.Where(p => p.Active == Active.Active && p.DeliveryDate.Year == selectedYear)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new SaleDeliveryViewModel
                    {
                        Id = _customDataProtector.Protect(p.Id.ToString()),
                        Customer = p.Sale.Customer.Name,
                        Number = p.Number,
                        Date = p.DeliveryDate,
                        Total = (int)p.TotalNet,
                        State = p.DeliveryState.ToString(),
                        SaleNumber = p.Sale.Number,
                        SaleId = _customDataProtector.Protect(p.Sale.Id.ToString())
                    })
                    .ToListAsync();

                dataTablesResult.recordsTotal = await _appDbContext.SaleDeliveries.Where(p => p.Active == Active.Active).CountAsync();

                return dataTablesResult;
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new SaleDeliveryRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new SaleDeliveryRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<IEnumerable<SaleDeliveryItem>> GetSaleDeliveryItemsById(int Id)
        {
            try 
            { 
                return await _appDbContext.SaleDeliveryItems.Where(a => a.SaleDeliveryId == Id && a.Active == Active.Active)
                    .Include(s => s.SaleDelivery)
                    .Include(i => i.SaleItem)
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
