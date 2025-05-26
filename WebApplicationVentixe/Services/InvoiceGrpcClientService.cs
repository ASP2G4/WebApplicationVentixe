using Google.Protobuf.WellKnownTypes;
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
}