namespace WebApplicationVentixe.Models.Events;

public class UpdateEventFormData
{
    public string Id { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public string EventLocation { get; set; } = null!;
    public DateTime EventDate { get; set; }
}
