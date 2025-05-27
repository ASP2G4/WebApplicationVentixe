using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Services;

namespace WebApplicationVentixe.Controllers;

public class BookingsController(BookingsService bookingsService) : Controller
{
    private readonly BookingsService _bookingsService = bookingsService;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Bookings";
        var bookings = await _bookingsService.GetAllBookingsAsync();
        return View(bookings);
    }
}
