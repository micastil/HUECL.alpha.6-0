using DocumentFormat.OpenXml.Office2010.Excel;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HUECL.alpha._6_0.Controllers
{
    public class SaleInvoicesController : Controller
    {

        private readonly AppDbContext _appDbcontext;
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleDeliveryRepository _saleDeliveryRepository;
        private readonly ISaleInvoiceRepository _saleInvoiceRepository;
        private readonly ILogger<SalesController> _logger;

        public SaleInvoicesController(AppDbContext appDbcontext,
            ILogger<SalesController> logger,
            ISaleRepository saleRepository,
            ISaleDeliveryRepository saleDeliveryRepository,
            ISaleInvoiceRepository saleInvoiceRepository)
        {
            _appDbcontext = appDbcontext;
            _logger = logger;

            _saleRepository = saleRepository;
            _saleDeliveryRepository = saleDeliveryRepository;
            _saleInvoiceRepository = saleInvoiceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> AddSaleInvoicePayment(int Id)
        {
            try 
            {
                var _invoice = await _saleInvoiceRepository.GetSaleInvoiceById(Id);
                if(_invoice == null)
                {
                    return BadRequest(new { msg = "No existe la factura en sistema. Intente nuevamente" });
                }
                SaleInvoicePayment _payment = new SaleInvoicePayment 
                {
                    SaleInvoiceId = _invoice.Id,
                    SaleInvoice = _invoice
                };
                return PartialView("_SaleInvoicePaymentCreate", _payment);
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
        public async Task<IActionResult> AddSaleInvoicePayment(SaleInvoicePayment saleInvoicePayment)
        {
            //TODO: Falta validar que la suma de pagos no sea mayor al Total Invoice
            try 
            {
                if (saleInvoicePayment.TotalPayment == 0) 
                {
                    return BadRequest(new { msg = "Debe ingresar un valor mayor a 0. Intente nuevamente" });
                }

                var _invoice = await _saleInvoiceRepository.GetSaleInvoiceById(saleInvoicePayment.SaleInvoiceId);
                
                if (_invoice == null) 
                {
                    return BadRequest(new { msg = "No existe la factura en sistema. Intente nuevamente" });
                }

                if(await _saleInvoiceRepository.AddSaleInvoicePayment(saleInvoicePayment) == 0) 
                {
                    return StatusCode(500, "Error al ingresar pago. Intente nuevamente");
                }

                var _result = await _saleInvoiceRepository.GetSaleInvoiceById(saleInvoicePayment.SaleInvoiceId);
                if (_result == null) 
                {
                    return BadRequest(new { msg = "No existe la factura en sistema. Intente nuevamente" });
                }

                return PartialView("_SaleInvoiceDetail", _result);

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

        [HttpGet]
        public async Task<IActionResult> AddSaleInvoice(int Id)
        {
            try
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var _delivery = await _saleDeliveryRepository.GetSaleDeliveryById(Id);

                    if (_delivery == null)
                    {
                        return StatusCode(500, new { status = DeliveryVerify.NotFound, message = "No se encuentra el Despacho en la Base de Datos" });
                    }

                    SaleInvoice _model = new SaleInvoice
                    {
                        SaleDeliveryId = _delivery.Id,
                        Date = DateTime.Now,
                        CreationDate = DateTime.Now
                    };

                    var partialViewString = await this.RenderViewToStringAsync("_SaleInvoiceCreate", _model);
                    return new JsonResult(new { status = DeliveryVerify.Found, partialView = partialViewString });

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
                    var _delivery = await _saleDeliveryRepository.GetSaleDeliveryById(model.SaleDeliveryId);

                    if (_delivery != null)
                    {
                        if (await _saleInvoiceRepository.AddSaleInvoice(model) > 0)
                        {
                            var _resultModel = await _saleDeliveryRepository.GetAllSaleDeliveriesBySaleId(_delivery.SaleId);
                            var _resultStatus = "";

                            if (_delivery.DeliveryState == DeliveryState.WithInvoice)
                            {
                                _resultStatus = "WithInvoice";
                            }

                            var partialViewString = await this.RenderViewToStringAsync("/Views/Shared/_SaleDeliveryList.cshtml", _resultModel);

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
        
        [HttpPost]
        public async Task<IActionResult> DeletePayment(int Id)
        {
            try
            {
                var _payment = await _saleInvoiceRepository.GetSaleInvoicePaymentById(Id);

                if (_payment != null)
                {
                    
                    if (await _saleInvoiceRepository.DeleteInvoicePaymet(Id) > 0)
                    {
                        return Ok();
                    }
                }
                return NotFound(new { item = Id, msg = "No se encuentra el Pago en la Base de Datos" });
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

        [HttpGet]
        public async Task<IActionResult> Detail(int Id)
        {
            try
            {
                if (await _saleInvoiceRepository.SaleInvoiceExistis(Id))
                {
                    var _result = await _saleInvoiceRepository.GetSaleInvoiceById(Id);
                    if (_result == null) { return NotFound(); }
                    return PartialView("_SaleInvoiceDetail", _result);
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

        [HttpGet]
        public async Task<IActionResult> GetAllSaleInvoicePayments(int Id)
        {
            try 
            {
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
