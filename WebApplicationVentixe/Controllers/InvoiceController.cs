using Microsoft.AspNetCore.Mvc;

namespace WebApplicationVentixe.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Invoices";
            return View();
        }
    }
}
