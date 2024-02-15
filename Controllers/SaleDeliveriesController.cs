using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HUECL.alpha._6_0.Controllers
{
    public class SaleDeliveriesController : Controller
    {
        private readonly AppDbContext _appDbcontext;
        private readonly ISaleDeliveryRepository _saleDeliveryRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;

        public SaleDeliveriesController(
            AppDbContext appDbcontext, 
            ISaleDeliveryRepository saleDeliveryRepository, 
            ISaleRepository saleRepository, 
            ILogger<SalesController> logger
            )
        {
            _appDbcontext = appDbcontext;
            _saleDeliveryRepository = saleDeliveryRepository;
            _saleRepository = saleRepository;
            _logger = logger;
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
                        var partialViewString = await this.RenderViewToStringAsync("/Views/Sales/_SaleDeliveryList.cshtml", _resultModel);

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
