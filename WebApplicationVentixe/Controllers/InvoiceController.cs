using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Services;

namespace WebApplicationVentixe.Controllers
{
    public class InvoiceController(InvoiceGrpcClientService invoiceService) : Controller
    {
        private readonly InvoiceGrpcClientService _invoiceService = invoiceService;

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Invoices";
            var invoices = await _invoiceService.GetInvoicesAsync();
            return View(invoices);
        }
    }
}