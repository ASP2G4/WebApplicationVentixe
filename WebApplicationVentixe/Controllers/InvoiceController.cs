using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Models.Invoice;
using WebApplicationVentixe.Protos.Invoice;
using WebApplicationVentixe.Services;

namespace WebApplicationVentixe.Controllers
{
    [Authorize (Roles ="Admin")]
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

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceDto = new InvoiceDetailsViewModel
            {
                Id = invoice.Id,
                StartDate = invoice.StartDate.ToDateTime(),
                EndDate = invoice.EndDate.ToDateTime(),
                UserName = "Username",
                UserAddress = "User Address",
                UserEmail = "User Email",
                UserPhone = "User Phone",
                CompanyName = "Company Name",
                CompanyAddress = "Company Address",
                CompanyEmail = "Company Email",
                CompanyPhone = "Company Phone",
                TicketCategory = "Ticket Category",
                TicketPrice = 120,
                TicketCount = 2,
                StatusName = invoice.StatusName
            };


            return PartialView("~/Views/Shared/Partials/InvoicePartials/InvoiceDetails.cshtml", invoiceDto);
        }
    }
}