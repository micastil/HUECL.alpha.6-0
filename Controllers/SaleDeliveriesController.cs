using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HUECL.alpha._6_0.Controllers
{
    public class SaleDeliveriesController : Controller
    {
        private readonly AppDbContext _appDbcontext;
        private readonly ISaleDeliveryRepository _saleDeliveryRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public SaleDeliveriesController(
            AppDbContext appDbcontext, 
            ISaleDeliveryRepository saleDeliveryRepository, 
            ISaleRepository saleRepository, 
            ILogger<SalesController> logger,
            UserManager<ApplicationUser> userManager
            )
        {
            _appDbcontext = appDbcontext;
            _saleDeliveryRepository = saleDeliveryRepository;
            _saleRepository = saleRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index() 
        {
            return View();
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
    }
}
