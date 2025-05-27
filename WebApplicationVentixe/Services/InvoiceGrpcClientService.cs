using Google.Protobuf.WellKnownTypes;
using WebApplicationVentixe.Models.Invoice;
using WebApplicationVentixe.Protos;
using WebApplicationVentixe.Protos.Invoice;

namespace WebApplicationVentixe.Services;

public class InvoiceGrpcClientService(InvoiceService.InvoiceServiceClient client)
{
    private readonly InvoiceService.InvoiceServiceClient _client = client;

    public async Task<List<Invoice>> GetInvoicesAsync()
    {        
        var response = await _client.GetInvoicesAsync(new Empty());
        return response.Invoices.ToList();
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        var response = await _client.GetInvoiceByIdAsync(new GetInvoiceByIdRequest
        {
            Id = id
        });

        return response?.Invoice;
    }

    public async Task<bool> UpdateInvoiceAsync(InvoiceUpdateModel model)
    {
        var request = new UpdateInvoiceRequest
        {
            Invoice = new Invoice
            {
                Id = model.Id,
                StartDate = Timestamp.FromDateTime(model.StartDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(model.EndDate.ToUniversalTime()),
                UserId = model.UserId,
                BookingId = model.BookingId,
                CompanyId = model.CompanyId,
                StatusId = model.StatusId,
                StatusName = model.StatusName
            }
        };
        UpdateInvoiceResponse response = await _client.UpdateInvoiceAsync(request);
        return response.Success;
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        var request = new DeleteInvoiceRequest
        {
            Id = id
        };
        DeleteInvoiceResponse response = await _client.DeleteInvoiceAsync(request);
        return response.Success;
    }
}