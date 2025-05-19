namespace WebApplicationVentixe.Models.Invoice;

public class InvoiceViewModel
{
    public int Id { get; set; }
    public string InvoiceNumber => $"INV100{Id}";

    public DateTime StartDate { get; set; }

    public decimal TicketPrice { get; set; }

    public string? StatusName { get; set; }
}
