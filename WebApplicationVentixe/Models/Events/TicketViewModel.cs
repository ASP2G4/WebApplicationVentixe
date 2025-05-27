namespace WebApplicationVentixe.Models.Events;

public class TicketViewModel
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;

    public int TicketAmount { get; set; }

    public double TicketPrice { get; set; }
    public TicketCategoryViewModel Category { get; set; } = null!;
}

public class TicketCategoryViewModel
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
}