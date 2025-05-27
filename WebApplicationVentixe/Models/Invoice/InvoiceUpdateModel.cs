namespace WebApplicationVentixe.Models.Invoice;

public class InvoiceUpdateModel
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string UserId { get; set; } = null!;

    public int BookingId { get; set; }

    public string CompanyId { get; set; } = null!;

    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;
}
