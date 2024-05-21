using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.Repositories;
using System.Globalization;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace HUECL.alpha._6_0.Controllers
{
    public class SalesController : Controller
    {
        private readonly AppDbContext _appDbcontext;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;

        public SalesController(AppDbContext appDbcontext, ISaleRepository saleRepository, ILogger<SalesController> logger)
        {
            _appDbcontext = appDbcontext;
            _saleRepository = saleRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSales()
        {
            try
            {
                var _draw = Request.Form["draw"].FirstOrDefault();
                var _start = Request.Form["start"].FirstOrDefault();
                var _length = Request.Form["length"].FirstOrDefault();
                var _searchValue = Request.Form["search[value]"].FirstOrDefault();
                var _selectedYear = int.Parse(Request.Form["currentYear"].FirstOrDefault());

                if (!int.TryParse(_start, out int _skip))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");
                if (!int.TryParse(_length, out int _pageSize))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");

                // Sort the data based on the selected column and direction
                var _sortColumnIndex = int.Parse(Request.Form["order[0][column]"].FirstOrDefault());
                var _sortColumnName = Request.Form[$"columns[{_sortColumnIndex}][data]"].FirstOrDefault();
                var _sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                DataTablesViewModel<SaleViewModel> result = await _saleRepository.GetDataTablesSale(
                    _draw,
                    _skip,
                    _pageSize,
                    _searchValue,
                    _sortColumnIndex,
                    _sortColumnName,
                    _sortDirection,
                    _selectedYear
                    );

                return Json(new
                {
                    draw = _draw,
                    recordsFiltered = result.recordsFiltered,
                    recordsTotal = result.recordsTotal,
                    data = result.Data
                });
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error en SalesController/Index: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error en SalesController/Index: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }

                var sale = await _saleRepository.GetSaleById(id.Value);

                if (sale == null)
                {
                    return NotFound();
                }
                ViewBag.ProductId = new SelectList(await _appDbcontext.Products.ToListAsync(), "Id", "Name");
                return View(sale);
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error en SalesController/Details: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error en SalesController/Details: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }

        }

        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _appDbcontext.Customers.ToListAsync(), "Id", "Name");
            ViewData["CurrencyId"] = new SelectList(await _appDbcontext.Currencies.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Date,Comment,CustomerId, CurrencyId")] Sale sale)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _saleRepository.AddSale(sale) > 0)
                    {
                        return RedirectToAction(nameof(Details), new { sale.Id });
                    }
                    else
                    {
                        return Problem("Entity set 'AppDbContext.Sales'  is null.");
                    }
                }
                ViewData["CustomerId"] = new SelectList(await _appDbcontext.Customers.ToListAsync(), "Id", "Name", sale.CustomerId);
                return View(sale);
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleItem([Bind("Id, CustomerCode, RequestedDelivery, UnitaryPrice, Quantity, ProductId, SaleId")] SaleItem saleItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _saleRepository.AddSaleItem(saleItem) > 0)
                    {
                        var list = await _saleRepository.GetAllSaleItemsBySaleId(saleItem.SaleId);
                        return PartialView("_SaleItemsList", list);
                    }
                }
                return RedirectToAction("Details", new { saleItem.SaleId });
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Detalle de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Detalle de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSaleItem(int Id)
        {
            try
            {
                if (await _saleRepository.SaleItemExist(Id))
                {
                    return Json(new { status = "OK" });
                }
                return Json(new { status = "notOK" });
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Detalle de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Detalle de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        private bool SaleExists(int id)
        {
            return (_appDbcontext.Sales?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Deliveries(int? Id)
        {
            try
            {
                if (!Id.HasValue)
                {
                    return NotFound();
                }

                var _list = await _saleRepository.GetAllSaleDeliveriesBySaleId(Id.Value);
                var _sale = await _saleRepository.GetSaleById(Id.Value);

                ViewBag.SaleId = Id.Value;
                ViewBag.SaleState = _sale.SaleState;

                return View(_list);
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error en SalesController/Deliveries: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error en SalesController/Deliveries: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleDelivery([Bind("Id, Number, DeliveryDate, Comment, SaleId")] SaleDelivery delivery)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _saleRepository.AddSaleDelivery(delivery) > 0)
                    {
                        var list = await _saleRepository.GetAllSaleDeliveriesBySaleId(delivery.SaleId);
                        return PartialView("_SaleDeliveryList", list);
                    }
                }
                return RedirectToAction("Details", new { delivery.SaleId });
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddSaleDeliveryItems(int? SaleDeliveryId)
        {
            try
            {
                if (!SaleDeliveryId.HasValue)
                {
                    return NotFound();
                }

                SaleDelivery _delivery = await _saleRepository.GetSaleDeliveryById(SaleDeliveryId.Value);

                if (_delivery == null) { return NotFound(); }

                if (_delivery.Sale.SaleItems == null)
                {
                    return NotFound();
                }

                return PartialView("_SaleDeliveryItemsCreate", await _saleRepository.GetItemsAvailableForDelivery(_delivery.SaleId, SaleDeliveryId.Value));
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error en SalesController/Details: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error en SalesController/Details: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleDeliveryItems(List<SaleDeliveryItemViewModel> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _saleRepository.AddSaleDeliveryItems(model) > 0)
                    {
                        if (model.FirstOrDefault() != null)
                        {
                            int saleDeliveryId = model.FirstOrDefault().SaleDeliveryId;
                            SaleDelivery delivery = await _saleRepository.GetSaleDeliveryById(saleDeliveryId);

                            var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(delivery.SaleId);
                            var _resultStatus = "";

                            if (delivery.Sale.SaleState == SaleState.CompleteDelivery)
                            {
                                _resultStatus = "CompleteDelivery";
                            }

                            var partialViewString = await this.RenderViewToStringAsync("_SaleDeliveryList", _resultModel);

                            return new JsonResult(new { Status = _resultStatus, PartialView = partialViewString });

                            //return PartialView("_SaleDeliveryList", await _saleRepository.GetAllSaleDeliveriesBySaleId(delivery.SaleId));
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Item Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Item Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSaleDeliveryItem(int Id)
        {
            try
            {
                SaleDeliveryItem _item = await _saleRepository.GetSaleDeliveryItemById(Id);

                if (_item != null)
                {
                    int _saleId = _item.SaleDelivery.Sale.Id;

                    if (await _saleRepository.DeleteSaleDeliveryItemById(_item.Id) > 0)
                    {
                        var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(_saleId);
                        var partialViewString = await this.RenderViewToStringAsync("_SaleDeliveryList", _resultModel);

                        return new JsonResult(new { status = "1", partialView = partialViewString });
                    }
                }

                return StatusCode(500, new { status = "error", message = "Unable to delete item." });
            }
            catch (SaleRepositoryCustomException ex)
            {
                _logger.LogInformation("Error al ingresar Item Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error al ingresar Item Despacho de Venta: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }
    }
}
