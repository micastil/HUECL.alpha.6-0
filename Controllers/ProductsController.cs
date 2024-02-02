using Microsoft.AspNetCore.Mvc;
using HUECL.alpha._6_0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace HUECL.alpha._6_0.Controllers
{
    //[Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly AppDbContext appDbContext;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IProductRepository _productRepository, AppDbContext _appDbContext, ILogger<ProductsController> _logger)
        {
            productRepository = _productRepository;
            appDbContext = _appDbContext;
            logger = _logger;
        }

        public async Task<IActionResult> IndexOld()
        {
            int totalProducts = await productRepository.TotalProducts();

            var viewModel = new PaginationViewModel<Product>
            {
                CurrentPage = 1,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)PageCustomSize.Normal)
            };

            return View(viewModel);
        }

        public IActionResult Index() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProducts()
        {
            try 
            {
                var _draw = Request.Form["draw"].FirstOrDefault();
                var _start = Request.Form["start"].FirstOrDefault();
                var _length = Request.Form["length"].FirstOrDefault();
                var _searchValue = Request.Form["search[value]"].FirstOrDefault();

                if (!int.TryParse(_start, out int _skip))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");
                if(!int.TryParse(_length, out int _pageSize))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");

                // Sort the data based on the selected column and direction
                var _sortColumnIndex = int.Parse(Request.Form["order[0][column]"].FirstOrDefault());
                var _sortColumnName = Request.Form[$"columns[{_sortColumnIndex}][data]"].FirstOrDefault();
                var _sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                DataTablesViewModel<ProductViewModel> result = await productRepository.GetDataTablesProduct(
                    _draw,
                    _skip,
                    _pageSize,
                    _searchValue,
                    _sortColumnIndex,
                    _sortColumnName,
                    _sortDirection
                    );

                return Json(new
                {
                    draw = _draw,
                    recordsFiltered = result.recordsFiltered,
                    recordsTotal = result.recordsTotal,
                    data = result.Data
                });
            }
            catch (ProductRepositoryCustomException ex)
            {
                logger.LogInformation("Error en Lista de Productos: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error en Lista de Productos: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }
    

    public async Task<IActionResult> LoadProducts(int page = 1, string searchTerm = "")
        {
            try 
            {
                var products = await productRepository.GetPageSearchProducts(page, (int)PageCustomSize.Normal, searchTerm);
                int totalProducts = await productRepository.TotalProducts();

                var viewModel = new PaginationViewModel<Product>
                {
                    Items = products,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)PageCustomSize.Normal)
                };

                return PartialView("_ProductListPartial", viewModel.Items);
            }
            catch (ProductRepositoryCustomException ex)
            {
                logger.LogInformation("Error en Lista de Productos: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                logger.LogInformation("Error en Lista de Productos: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        public async Task<IActionResult> Create()
        {

            List<SelectListItem> selectListProvider = new  List<SelectListItem>{ 
                new SelectListItem { Text = "Seleccione Proveedor", Value = ""}
            };

            var providerList = await appDbContext.Providers.ToListAsync();
            selectListProvider.AddRange(providerList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString()}));

            List<SelectListItem> selectListCategory = new List<SelectListItem>{
                new SelectListItem { Text = "Seleccione Categoria", Value = ""}
            };

            var categoryList = await appDbContext.Categories.ToListAsync();
            selectListCategory.AddRange(categoryList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }));

            List<SelectListItem> selectListUnit = new List<SelectListItem>{
                new SelectListItem { Text = "Seleccione Unidad", Value = ""}
            };

            var unitList = await appDbContext.Units.ToListAsync();
            selectListUnit.AddRange(unitList.Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }));

            ViewBag.SelectListProvider = selectListProvider;
            ViewBag.SelectListCategory = selectListCategory;
            ViewBag.SelectListUnit = selectListUnit;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id, Name, InternalCode, ManufacturerCode, Description, ProviderId, SubCategoryId, UnitId")] 
            Product newProduct) 
        {
            if(ModelState.IsValid)
            {
                var result = await productRepository.AddProduct(newProduct);

                if (result > 0)
                {
                    return RedirectToAction("Index");
                }
                else 
                {
                    //TODO July 2022. Handle error page
                    return RedirectToAction("ProductError");
                }
            }
            return View();
        }

        public async Task<IActionResult> Details(string InternalCode)
        {
            var ProductForDetail = await productRepository.GetProductByCode(InternalCode);

            if (ProductForDetail != null)
            {
                return View(ProductForDetail);   
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetSubCategories(int IdCategory)
        {
            var items = await appDbContext.SubCategories.Where(c => c.CategoryId == IdCategory).ToListAsync();

            var jsonItems = items.Select(item => new
            {
                Value = item.Id,
                Text = item.Name
            });

            return Json(jsonItems);
        }

        public async Task<IActionResult> GetFile() 
        {
            using (var workbook = new XLWorkbook())
            {
                var productsSheet = workbook.Worksheets.Add("Products");
                productsSheet.Cell("A1").Value = "Nombre Producto";
                productsSheet.Cell("B1").Value = "Codigo Interno";
                productsSheet.Cell("C1").Value = "Codigo Fabricante";
                productsSheet.Cell("D1").Value = "Proveedor";
                productsSheet.Cell("E1").Value = "Sub Categoria";
                productsSheet.Cell("F1").Value = "Unidad";
                productsSheet.Cell("G1").Value = "Descripcion";

                var providerReferenceSheet = workbook.Worksheets.Add("Proveedores");
                providerReferenceSheet.Cell("A1").Value = "Nombre Proveedor";
                providerReferenceSheet.Cell("B1").Value = "Id Proveedor";

                var subcategoryReferenceSheet = workbook.Worksheets.Add("Sub Categorias");
                subcategoryReferenceSheet.Cell("A1").Value = "Nombre Sub Categoria";
                subcategoryReferenceSheet.Cell("B1").Value = "Id Sub Categoria";

                var unitReferenceSheet = workbook.Worksheets.Add("Unidades");
                unitReferenceSheet.Cell("A1").Value = "Nombre Unidad";
                unitReferenceSheet.Cell("B1").Value = "Id Unidad";

                var providers = await appDbContext.Providers.ToListAsync();
                var subCategories = await appDbContext.SubCategories.ToListAsync();
                var units = await appDbContext.Units.ToListAsync();

                for (int i = 0; i < providers.Count; i++)
                {
                    providerReferenceSheet.Cell(i + 2, 1).Value = providers[i].Name;
                    providerReferenceSheet.Cell(i + 2, 2).Value = providers[i].Id;
                }

                for (int i = 0; i < subCategories.Count; i++)
                {
                    subcategoryReferenceSheet.Cell(i + 2, 1).Value = subCategories[i].Name;
                    subcategoryReferenceSheet.Cell(i + 2, 2).Value = subCategories[i].Id;
                }

                for (int i = 0; i < units.Count; i++)
                {
                    unitReferenceSheet.Cell(i + 2, 1).Value = units[i].Name;
                    unitReferenceSheet.Cell(i + 2, 2).Value = units[i].Id;
                }

                // Save the Excel package to a stream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);

                // Prepare the stream for download
                stream.Position = 0;
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "product_template.xlsx";

                return File(stream, contentType, fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFIle(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index");
            }

            using (var stream = file.OpenReadStream())
            using (var workbook = new XLWorkbook(stream))
            {
                var productsSheet = workbook.Worksheet("Products");

                var products = productsSheet
                    .RowsUsed()
                    .Skip(1)
                    .Select(row => new Product
                    {
                        Name = row.Cell(1).Value.ToString(),
                        InternalCode = (int)(row.Cell(2).Value),
                        ManufacturerCode = row.Cell(3).Value.ToString(),
                        ProviderId = (int)row.Cell(4).Value,
                        SubCategoryId = (int)row.Cell(5).Value,
                        UnitId = (int)row.Cell(6).Value,
                        Description = row.Cell(7).Value.ToString()
                    })
                    .ToList();

                foreach (Product item in products)
                {
                    await productRepository.AddProduct(item);
                }

            }

            return RedirectToAction("Index");
        }

        public IActionResult UploadFile() { return View(); }
    }
}
