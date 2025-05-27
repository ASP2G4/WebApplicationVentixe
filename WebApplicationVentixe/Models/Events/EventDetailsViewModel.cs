namespace WebApplicationVentixe.Models.Events;

public class EventDetailsViewModel
{
    public EventsViewModel Event { get; set; } = null!;
    public List<TicketViewModel> Ticket { get; set; } = null!;
}
