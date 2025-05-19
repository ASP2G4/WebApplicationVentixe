using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Models.Invoice;
using WebApplicationVentixe.Services;

namespace WebApplicationVentixe.Controllers
{
    public class InvoiceController(InvoiceGrpcClientService invoiceService) : Controller
    {
        private readonly InvoiceGrpcClientService _invoiceService = invoiceService;

        public async Task<IActionResult> Index(string? input)
        {
            ViewData["Title"] = "Invoices";
            ViewBag.Input = input;

            var invoices = await _invoiceService.GetInvoicesAsync();

            var invoiceDto = invoices.Select(i => new InvoiceViewModel
            {
                Id = i.Id,
                StartDate = i.StartDate.ToDateTime(),
                StatusName = i.StatusName
            }).ToList();

            if (!string.IsNullOrWhiteSpace(input))
            {
                invoiceDto = invoiceDto.Where(i =>                     
                    i.InvoiceNumber.Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(invoiceDto);
        }
    }
}