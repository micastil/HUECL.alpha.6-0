using HUECL.alpha._6_0.Models;
using Microsoft.AspNetCore.Mvc;

namespace HUECL.alpha._6_0.Controllers
{
    public class SaleInvoicesController : Controller
    {

        private readonly AppDbContext _appDbcontext;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;

        public SaleInvoicesController (AppDbContext appDbcontext, ISaleRepository saleRepository, ILogger<SalesController> logger)
        {
            _appDbcontext = appDbcontext;
            _saleRepository = saleRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> AddSaleInvoice(int Id)
        {
            try
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    SaleDelivery _delivery = await _saleRepository.GetSaleDeliveryById(Id);
                    if (_delivery != null)
                    {
                        SaleInvoice _model = new SaleInvoice
                        {
                            SaleDeliveryId = _delivery.Id,
                            Date = DateTime.Now,
                            CreationDate = DateTime.Now
                        };

                        var partialViewString = await this.RenderViewToStringAsync("_SaleInvoiceCreate", _model);
                        return new JsonResult(new { status = DeliveryVerify.Found, partialView = partialViewString });
                    }
                    else
                    {
                        return StatusCode(500, new { status = DeliveryVerify.NotFound, message = "No se encuentra el Despacho en la Base de Datos" });
                    }
                }

                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleInvoice([Bind("Number, Date, PaymentDate, Comment, SaleDeliveryId")] SaleInvoice model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SaleDelivery _delivery = await _saleRepository.GetSaleDeliveryById(model.SaleDeliveryId);

                    if (_delivery != null)
                    {
                        if (await _saleRepository.AddSaleInvoice(model) > 0)
                        {
                            var _resultModel = await _saleRepository.GetAllSaleDeliveriesBySaleId(_delivery.SaleId);
                            var _resultStatus = "";

                            if (_delivery.DeliveryState == DeliveryState.WithInvoice)
                            {
                                _resultStatus = "WithInvoice";
                            }

                            var partialViewString = await this.RenderViewToStringAsync("/Views/Sales/_SaleDeliveryList.cshtml", _resultModel);

                            return new JsonResult(new { status = _resultStatus, partialView = partialViewString });
                        }
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Ocurrio un error al iingresar los datos" });
                    }
                }

                return StatusCode(500, new { message = "Ocurrio un error al iingresar los datos" });

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
