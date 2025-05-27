using WebApplicationVentixe.Models.Booking;

namespace WebApplicationVentixe.Services;

public class BookingsService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<BookingViewModel>?> GetAllBookingsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://asp2ventixe-hydce2e3guamhacr.swedencentral-01.azurewebsites.net/api/bookings");
        request.Headers.Add("x-api-key", "0c96559449514d8eaabe31e116f5be2a");

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
