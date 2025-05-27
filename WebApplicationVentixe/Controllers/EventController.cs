using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebApplicationVentixe.Models.Events;

namespace WebApplicationVentixe.Controllers;

public class EventController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _eventApiUrl;
    private readonly string _ticketApiUrl;

    public EventController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _eventApiUrl = configuration["ApiSettings:EventAPI"]!;
        _ticketApiUrl = configuration["ApiSettings:TicketAPI"]!;

    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Events";

        var data = await _httpClient.GetFromJsonAsync<List<EventsViewModel>>(_eventApiUrl);
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddEventFormData formData)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        try
        {
            var json = JsonSerializer.Serialize(formData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_eventApiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "API Error: Failed to create event.");
        }
        
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
        }

        return View("Index");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateEventFormData formData)
    {
        if (!ModelState.IsValid)
            return View("Index");

        try
        {
            var json = JsonSerializer.Serialize(formData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_eventApiUrl, content);
            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "API Error: Failed to update event.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
        }

        return View("Index");
    }

    public async Task<IActionResult> EventDetails(string id)
    {
        var eventDetail = await _httpClient.GetFromJsonAsync<EventsViewModel>($"{_eventApiUrl}/{id}");
        if (eventDetail == null)
            return NotFound();

        var tickets = await _httpClient.GetFromJsonAsync<List<TicketViewModel>>($"{_ticketApiUrl}/{id}");

        // AI kod
        var fullInfo = new EventDetailsViewModel
        {
            Event = eventDetail,
            Ticket = tickets!
        };

        return View(fullInfo);
    }
}
