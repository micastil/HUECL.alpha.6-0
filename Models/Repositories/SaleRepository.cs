using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(AppDbContext appDbContext, ILogger<SaleRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<int> AddSale(Sale entity)
        {
            try 
            {
                entity.Active = Active.Active;
                entity.CreationDate = DateTime.Now;
                entity.SaleState = SaleState.NoDelivery;

                _appDbContext.Sales.Add(entity);
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

        public async Task<int> AddSaleItem(SaleItem entity)
        {
            try 
            {
                entity.Active = Active.Active;
                entity.CreationDate = DateTime.Now;
                entity.SubTotal = entity.UnitaryPrice * entity.Quantity;

                var _Sale = await _appDbContext.Sales.Where(i => i.Id == entity.SaleId).FirstOrDefaultAsync();
                if (_Sale != null)
                {
                    _Sale.Total += entity.SubTotal;

                    _appDbContext.SaleItems.Add(entity);

                    return await _appDbContext.SaveChangesAsync();
                }
                else 
                {
                    return 0;
                }
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

        public Task<int> DeleteSaleItem(int SaleItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Sale>> GetAllSales()
        {
            try 
            {
                return await _appDbContext.Sales.
                    Include(c => c.Customer).
                    Where(t => t.Active == Active.Active).
                    ToListAsync();
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

        public async Task<Sale> GetSaleById(int id)
        {
            try
            {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return await _appDbContext.Sales.
                    Include(c => c.Customer).
                    Include(p => p.Currency).
                    Include(s => s.SaleItems).
                    Where(t => t.Active == Active.Active && t.Id == id).
                    FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
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

        public async Task<SaleItem> GetSaleItemById(int id) 
        {
            try
            {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return await _appDbContext.SaleItems.
                    Where(t => t.Active == Active.Active && t.Id == id).
                    FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
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

        public async Task<IEnumerable<SaleItem>> GetAllSaleItemsBySaleId(int SaleId)
        {
            try
            {
                return await _appDbContext.SaleItems.
                    Where(i => i.SaleId == SaleId && i.Active == Active.Active).
                    Include(p => p.Product).
                    ToListAsync();
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

        public async Task<bool> SaleItemExist(int SaleItemId)
        {
            try
            {
                return await _appDbContext.
                    SaleItems.
                    AnyAsync(p => p.Id == SaleItemId && p.Active == Active.Active);
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

        public async Task<bool> SaleItemHasDelivery(int SaleItemId)
        {
            try
            {
                return await _appDbContext.
                    SaleItems.
                    AnyAsync(p => p.Id == SaleItemId && p.Active == Active.Active);
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
                    .Include(s => s.Sale)
                        .ThenInclude(c => c.Customer)
                    .Include(x => x.SaleInvoice)
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

        public async Task<int> AddSaleDelivery(SaleDelivery entity)
        {
            try
            {
                entity.TotalDelivery = 0;
                entity.TotalNet = 0;
                entity.TotalTax = 0;
                entity.CreationDate = DateTime.Now;
                entity.Active = Active.Active;
                entity.DeliveryState = DeliveryState.Empty;

                _appDbContext.SaleDeliveries.Add(entity);
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

        public async Task<SaleDelivery> GetSaleDeliveryById(int SaleDeliveryId)
        {
            try
            {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
                return await _appDbContext.SaleDeliveries.
                    Include(s => s.Sale).
                    Include(i => i.SaleInvoice).
                    Include(i => i.Sale.SaleItems).
                        ThenInclude(p => p.Product).
                    Where(t => t.Active == Active.Active && t.Id == SaleDeliveryId).
                    FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
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

        public async Task<int> AddSaleDeliveryItems(ICollection<SaleDeliveryItemViewModel> _list)
        {
            try
            {
                if (_list != null && _list.Count > 0)
                {
                    var _deliveryId = _list.FirstOrDefault().SaleDeliveryId;

                    decimal _totalDelivery = 0;
                    decimal _totalOverAll = 0;

                    SaleDelivery _saleDelivery = await GetSaleDeliveryById(_deliveryId);
                    Sale _sale = await GetSaleById(_saleDelivery.SaleId);

                    var _allDeliveries = await GetAllSaleDeliveriesBySaleId(_sale.Id);

                    // Total Delivery for all Deliveries on the DB
                    foreach (SaleDelivery _delivery in _allDeliveries) 
                    {
                        _totalOverAll += _delivery.TotalNet;
                    }

                    foreach (SaleDeliveryItemViewModel item in _list)
                    {
                        if (item.Id != 0)
                        {
                            SaleDeliveryItem _deliveryItem = new()
                            {
                                Active = Active.Active,
                                CreationDate = DateTime.Now,
                                Quantity = item.Quantity,
                                SaleDeliveryId = item.SaleDeliveryId,
                                SaleItemId = item.Id
                            };

                            
                            SaleItem _saleItem = await GetSaleItemById(item.Id);
                            

                            _deliveryItem.SubTotal = item.Quantity * _saleItem.UnitaryPrice;

                            _saleDelivery.TotalNet += _saleItem.UnitaryPrice * item.Quantity;
                            _saleDelivery.TotalTax = (int)(_saleDelivery.TotalNet * (Decimal)IVARate.CL);
                            _saleDelivery.TotalDelivery = (int)(_saleDelivery.TotalNet + _saleDelivery.TotalTax);

                            _saleDelivery.DeliveryState = DeliveryState.WithItems;

                            //if (_sale.Total == _saleDelivery.TotalNet)
                            //{
                            //    _sale.SaleState = SaleState.CompleteDelivery;
                            //}
                            //else
                            //{
                            //    _sale.SaleState = SaleState.PartialDelivery;
                            //}

                            _totalDelivery += _deliveryItem.SubTotal;

                            _appDbContext.SaleDeliveryItems.Add(_deliveryItem);
                        }
                    }

                    _totalOverAll += _totalDelivery;

                    if (_sale.Total == _totalOverAll)
                    {
                        _sale.SaleState = SaleState.CompleteDelivery;
                    }
                    else 
                    {
                        _sale.SaleState = SaleState.PartialDelivery;
                    }

                    return await _appDbContext.SaveChangesAsync();
                }
                else 
                {
                    return 0;
                }
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

        public async Task<IEnumerable<SaleDeliveryItem>> GetAllSaleDeliveryItemsByDeliveryId(int SaleDeliveryId)
        {
            try 
            {
                return await _appDbContext.SaleDeliveryItems.
                    Where(t => t.SaleDeliveryId == SaleDeliveryId && t.Active == Active.Active).
                    Include(i => i.SaleItem).ThenInclude(p => p.Product).
                    ToListAsync();
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

        public async Task<DataTablesViewModel<SaleViewModel>> GetDataTablesSale(
            string? draw, 
            int skip, 
            int pageSize, 
            string? searchValue, 
            int sortColumnIndex, 
            string? sortColumnName, 
            string? sortDirection,
            int selectedYear)
        {
            try
            {
                DataTablesViewModel<SaleViewModel> dataTablesResult = new DataTablesViewModel<SaleViewModel>();

                var query = _appDbContext.Sales.Include(p=> p.Customer).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(p =>
                        (
                        p.Customer.Name.Contains(searchValue) ||
                        p.Number.Contains(searchValue)
                        )
                    );
                }

                dataTablesResult.recordsFiltered = await query.CountAsync();

                switch (sortColumnName)
                {
                    case "customer":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Customer.Name)
                            : query.OrderByDescending(p => p.Customer.Name);
                        break;
                    case "number":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Number)
                            : query.OrderByDescending(p => p.Number);
                        break;
                    case "date":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Date)
                            : query.OrderByDescending(p => p.Date);
                        break;
                    case "total":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Total)
                            : query.OrderByDescending(p => p.Total);
                        break;
                        // Add cases for other sortable columns as needed
                }

                dataTablesResult.Data = await query.Where(p => p.Active == Active.Active && p.Date.Year == selectedYear)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new SaleViewModel
                    {
                        Id = p.Id,
                        Customer = p.Customer.Name,
                        Number = p.Number,
                        Date = p.Date,
                        Total = p.Total,
                        State = p.SaleState.ToString()
                    })
                    .ToListAsync();

                dataTablesResult.recordsTotal = await _appDbContext.Sales.Where(p => p.Active == Active.Active).CountAsync();

                return dataTablesResult;
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

        public async Task<bool> CheckQuantityDeliveryItem(SaleDeliveryItem entity)
        {
            SaleItem _saleItem = await _appDbContext.SaleItems.
                    Where(t => t.Active == Active.Active && t.Id == entity.SaleItemId).
                    FirstOrDefaultAsync();

            if (_saleItem == null)
            {
                return false;
            }

            IEnumerable<SaleDeliveryItem> _list = await _appDbContext.SaleDeliveryItems.
                Where(t => t.Active == Active.Active && t.SaleDeliveryId == entity.SaleDeliveryId && t.SaleItemId == entity.SaleItemId).
                ToListAsync();

            int _totalDelivery = 0;

            foreach (var item in _list)
            {
                _totalDelivery += item.Quantity;
            }

            _totalDelivery += entity.Quantity;

            if (_saleItem.Quantity >= _totalDelivery)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ICollection<SaleDeliveryItemViewModel>> GetItemsAvailableForDelivery(int SaleId, int SaleDeliveryId)
        {
            try 
            {
                //Lista que contiene el resultado
                ICollection<SaleDeliveryItemViewModel> _list = new List<SaleDeliveryItemViewModel>();

                //Lista que contiene los items de venta de la OC
                IEnumerable<SaleItem> _saleItems = await _appDbContext.SaleItems.Where(t => t.SaleId == SaleId && t.Active == Active.Active).ToListAsync();

                //Para cada item de la lista, debo verificar cuantos despachos existen
                foreach (SaleItem item in _saleItems)
                {
                    int _deliveryCount = 0;
                    IEnumerable<SaleDeliveryItem> _deliveredItems = await _appDbContext.SaleDeliveryItems.Where(t => t.SaleItemId == item.Id).ToListAsync();

                    foreach (var deliveryItem in _deliveredItems)
                    {
                        _deliveryCount += deliveryItem.Quantity;
                    }

                    if (_deliveryCount < item.Quantity)
                    {
                        SaleDeliveryItemViewModel _available = new SaleDeliveryItemViewModel();
                        _available.Id = item.Id;
                        _available.Name = item.Product.Name;
                        _available.Quantity = item.Quantity - _deliveryCount;
                        _available.SaleDeliveryId = SaleDeliveryId;
                        _list.Add(_available);
                    }
                }

                return _list;
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

        public async Task<SaleDeliveryItem> GetSaleDeliveryItemById(int SaleDeliveryItemId)
        {
            try 
            { 
                return await _appDbContext.SaleDeliveryItems.
                    Where(t => t.Id == SaleDeliveryItemId).
                    Include(s => s.SaleDelivery).ThenInclude(r => r.Sale).
                    FirstOrDefaultAsync();
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
                SaleDeliveryItem _deliveryItemtoDelete = await _appDbContext.SaleDeliveryItems.Where(t => t.Id == Id).FirstOrDefaultAsync();

                if (_deliveryItemtoDelete != null) 
                {
                    _appDbContext.SaleDeliveryItems.Remove(_deliveryItemtoDelete);
                    _deleteResult = await _appDbContext.SaveChangesAsync();

                    if (_deleteResult > 0) 
                    { 

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

    }
}
