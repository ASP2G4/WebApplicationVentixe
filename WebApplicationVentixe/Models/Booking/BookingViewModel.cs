namespace WebApplicationVentixe.Models.Booking;

public class BookingViewModel
{
    public int Id { get; set; }
    public int Tickets { get; set; }
    public DateTime CreatedAt { get; private set; }
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public bool Invoiced { get; set; }
    public bool Paid { get; set; }
    public bool Cancelled { get; set; }
}
