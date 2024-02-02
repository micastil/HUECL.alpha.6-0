using DocumentFormat.OpenXml.Wordprocessing;
using HUECL.alpha._6_0.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext appDbContext, ILogger<ProductRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<int> AddProduct(Product newProduct)
        {
            try 
            {
                newProduct.Active = (int)Active.Active;
                newProduct.CreationDate = DateTime.Now;
                newProduct.UnitaryCost = 0;

                _appDbContext.Products.Add(newProduct);

                return await _appDbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                return await _appDbContext.Products.
                Include(c => c.SubCategory).
                Include(c => c.SubCategory.Category).
                Include(p => p.Provider).
                Include(u => u.Unit).
                Where(a => a.Active == (int)Active.Active).
                ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<DataTablesViewModel<ProductViewModel>> GetDataTablesProduct(
            string? draw, 
            int skip, 
            int pageSize,
            string? searchValue,
            int sortColumnIndex, 
            string? sortColumnName, 
            string? sortDirection)
        {
            try 
            {
                DataTablesViewModel<ProductViewModel> dataTablesResult = new DataTablesViewModel<ProductViewModel>();

                var query = _appDbContext.Products.Include(p => p.Unit).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(p =>
                        (
                        p.Name.Contains(searchValue) ||
                        p.InternalCode.ToString().Contains(searchValue) ||
                        p.ManufacturerCode.Contains(searchValue)
                        )
                    );
                }

                dataTablesResult.recordsFiltered = await query.CountAsync();

                switch (sortColumnName)
                {
                    case "name":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Name)
                            : query.OrderByDescending(p => p.Name);
                        break;
                    case "internalCode":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.InternalCode)
                            : query.OrderByDescending(p => p.InternalCode);
                        break;
                    case "manufacturerCode":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.ManufacturerCode)
                            : query.OrderByDescending(p => p.ManufacturerCode);
                        break;
                        // Add cases for other sortable columns as needed
                }

                dataTablesResult.Data = await query.Where(p => p.Active == (int)Active.Active)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new ProductViewModel
                    {
                        InternalCode = p.InternalCode.ToString(),
                        Name = p.Name,
                        ManufacturerCode = p.ManufacturerCode,
                        Unit = p.Unit.Name
                    })
                    .ToListAsync();

                dataTablesResult.recordsTotal = await _appDbContext.Products.Where(p => p.Active == (int)Active.Active).CountAsync();

                return dataTablesResult;

            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetPageSearchProducts(int page, int pageSize, string searchTerm)
        {
            try 
            {
                return await _appDbContext.Products
                    .Where(t => t.Active == (int)Active.Active)
                    .OrderBy(p => p.InternalCode)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<Product> GetProductByCode(string internalcode)
        {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
            return await _appDbContext.Products.
                Include(c => c.SubCategory).
                Include(c => c.SubCategory.Category).
                Include(p => p.Provider).
                Include(u => u.Unit).
                Where(a => a.Active == (int)Active.Active && a.InternalCode == int.Parse(internalcode)).
                FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
        }

        public async Task<Product> GetProductById(int ProductId)
        {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
            return await _appDbContext.Products.
                Include(c => c.SubCategory).
                Include(c => c.SubCategory.Category).
                Include(p => p.Provider).
                Include(u => u.Unit).
                Where(a => a.Active == (int)Active.Active && a.Id == ProductId).
                FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
        }

        public async Task<bool> IsValid(int id)
        {
            return await _appDbContext.Products.
                Where(i => i.Id == id && i.Active == (int)Active.Active).
                AnyAsync();
        }

        public async Task<int> TotalProducts()
        {
            try 
            {
                return await _appDbContext.Products.CountAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Db Exception: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error: {mensaje}", ex.Message);
                throw new ProductRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public Task<int> UpdateProduct(int id, string comment)
        {
            throw new NotImplementedException();
        }
    }
}
