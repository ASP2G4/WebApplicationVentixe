using Microsoft.AspNetCore.Mvc;

namespace WebApplicationVentixe.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Bookings";
            return View();
        }
    }
}
