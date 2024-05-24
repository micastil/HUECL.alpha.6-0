using DocumentFormat.OpenXml.Office2010.Excel;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.CustomExceptions;
using HUECL.alpha._6_0.Models.Repositories;
using HUECL.alpha._6_0.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace HUECL.alpha._6_0.Controllers
{
    [Authorize]
    public class SaleDeliveriesController : Controller
    {
        
        private readonly ISaleDeliveryRepository _saleDeliveryRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;
        private readonly ICustomDataProtector _customDataProtector;
        private readonly UserManager<ApplicationUser> _userManager;

        public SaleDeliveriesController(
            ISaleDeliveryRepository saleDeliveryRepository, 
            ISaleRepository saleRepository, 
            ILogger<SalesController> logger,
            ICustomDataProtector customDataProtector,
            UserManager<ApplicationUser> userManager
            )
        {
            _saleDeliveryRepository = saleDeliveryRepository;
            _saleRepository = saleRepository;
            _logger = logger;
            _customDataProtector = customDataProtector;
            _userManager = userManager;
        }

        [Authorize(Policy = "CanRead")]
        [HttpGet]
        public IActionResult Index() 
        {
            return View();
        }

        [Authorize(Policy = "CanRead")]
        [HttpPost]
        public async Task<IActionResult> GetSaleDeliveries() 
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

                DataTablesViewModel<SaleDeliveryViewModel> result = await _saleDeliveryRepository.GetDataTablesSaleDelivery(
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
            catch (SaleDeliveryRepositoryCustomException ex)
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

        [Authorize(Policy = "CanDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteSaleDeliveryItem(int Id)
        {
            try
            {
                SaleDeliveryItem _item = await _saleRepository.GetSaleDeliveryItemById(Id);

                if (_item != null)
                {
                    int _saleId = _item.SaleDelivery.Sale.Id;

                    if (await _saleDeliveryRepository.DeleteSaleDeliveryItemById(_item.Id) > 0)
                    {
                        var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(_saleId);
                        var partialViewString = await this.RenderViewToStringAsync("/Views/Shared/_SaleDeliveryList.cshtml", _resultModel);

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

        [Authorize(Policy = "CanDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteSaleDelivery(int Id)
        {
            try 
            {
                if (await _saleDeliveryRepository.SaleDeliveryExists(Id))
                {
                    var _delivery = await _saleDeliveryRepository.GetSaleDeliveryById(Id);
                    
                    if (_delivery == null) 
                    {
                        return NotFound();
                    }

                    if (await _saleDeliveryRepository.DeleteSaleDelivery(_delivery) == 0) 
                    {
                        return StatusCode(500, "Ha ocurrido un error en la aplicacion");
                    }

                    string _resultStatus = "0";

                    if (_delivery.Sale.SaleState == SaleState.PartialDelivery || _delivery.Sale.SaleState == SaleState.NoDelivery) 
                    {
                        _resultStatus = "1";
                    }

                    return new JsonResult(new { status = _resultStatus });
                }
                
                return NotFound();
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

        [Authorize(Policy = "CanRead")]
        [HttpGet]
        public async Task<IActionResult> Details(string Id)
        {
            try 
            {
                if (Id == null || Id == string.Empty)
                {
                    return NotFound();
                }

                int unprotected = int.Parse(_customDataProtector.Unprotect(Id));

                var result = await _saleDeliveryRepository.GetSaleDeliveryById(unprotected);
                
                if (result == null) { return NotFound(); }

                return View(result);
            }
            catch (CryptographicException ex)
            {
                _logger.LogInformation("Error en Cryptografico SalesController/Details: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (SaleDeliveryRepositoryCustomException ex)
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

        [Authorize(Policy = "CanWrite")]
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

        [Authorize(Policy = "CanWrite")]
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
                        var first = model.FirstOrDefault();

                        if (first != null)
                        {
                            int saleDeliveryId = first.SaleDeliveryId;

                            SaleDelivery delivery = await _saleRepository.GetSaleDeliveryById(saleDeliveryId);

                            var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(delivery.SaleId);
                            var _resultStatus = "";

                            if (delivery.Sale.SaleState == SaleState.CompleteDelivery)
                            {
                                _resultStatus = "CompleteDelivery";
                            }

                            var partialViewString = await this.RenderViewToStringAsync("_SaleDeliveryList", _resultModel);

                            return new JsonResult(new { Status = _resultStatus, PartialView = partialViewString });
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

        [Authorize(Policy = "CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleDeliveryItemsOnDetails(List<SaleDeliveryItemViewModel> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _saleRepository.AddSaleDeliveryItems(model) > 0)
                    {
                        var first = model.FirstOrDefault();

                        if (first != null)
                        {
                            int saleDeliveryId = first.SaleDeliveryId;

                            SaleDelivery delivery = await _saleRepository.GetSaleDeliveryById(saleDeliveryId);

                            var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(delivery.SaleId);
                            var _resultStatus = "";

                            if (delivery.Sale.SaleState == SaleState.CompleteDelivery)
                            {
                                _resultStatus = "CompleteDelivery";
                            }

                            var partialViewString = await this.RenderViewToStringAsync("_SaleDeliveryList", _resultModel);

                            return new JsonResult(new { Status = _resultStatus, PartialView = partialViewString });
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
    }
}
