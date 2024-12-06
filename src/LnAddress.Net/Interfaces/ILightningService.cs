namespace LnAddress.Net.Interfaces;

public interface ILightningService
{
    Task<string> FetchInvoiceAsync(long valueMillisats, string username, string? comment);
    Task<bool> CheckConnection();
}