using WebApplicationVentixe.Models.Booking;

namespace WebApplicationVentixe.Services;

public class BookingsService(HttpClient httpClient, IConfiguration config)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiUrl = config["BookingsService:ConnectionString"]!;
    private readonly string _apiKey = config["BookingsService:SecretKey"]!;

    public async Task<List<BookingViewModel>?> GetAllBookingsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
        request.Headers.Add("x-api-key", _apiKey);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        } 

        var content = await response.Content.ReadAsStringAsync();
        var bookings = System.Text.Json.JsonSerializer.Deserialize<List<BookingViewModel>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return bookings;
    }
}
