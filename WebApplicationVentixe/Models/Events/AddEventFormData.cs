namespace WebApplicationVentixe.Models.Events;

public class AddEventFormData
{
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public string EventLocation { get; set; } = null!;


    public DateTime EventDate { get; set; }

    public int SilverTicketAmount { get; set; }
    public int SilverTicketPrice { get; set; }
    public int GoldTicketAmount { get; set; }
    public int GoldTicketPrice { get; set; }
}
