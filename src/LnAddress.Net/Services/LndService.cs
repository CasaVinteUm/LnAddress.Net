using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Net.Client;
using Lnrpc;

namespace LnAddress.Net.Services;

using Interfaces;

public class LndService : ILightningService
{
    private readonly Lightning.LightningClient _rpcClient;
    private readonly string _macaroon;
    private readonly ILogger<LndService> _logger;

    public LndService(IConfiguration configuration, ILogger<LndService> logger)
    {
        _logger = logger;

        var macaroonBytes = Convert.FromBase64String(configuration["Lnd:Macaroon"]
                                                     ?? throw new Exception("Lnd macaroon config is missing"));
        _macaroon = BitConverter.ToString(macaroonBytes).Replace("-", ""); // hex format stripped of "-" chars

        var rawCert = Convert.FromBase64String(configuration["Lnd:Cert"]
                                               ?? throw new Exception("Lnd certificate config is missing"));
        var x509Cert = new X509Certificate2(rawCert);
        var httpClientHandler = new HttpClientHandler
        {
            // Validating a self-signed cert won't work. Therefore, validate the certificate directly
            ServerCertificateCustomValidationCallback = (_, cert, _, _) => x509Cert.Equals(cert)
        };

        var credentials = ChannelCredentials.Create(new SslCredentials(), CallCredentials.FromInterceptor(AddMacaroon));

        var channel = GrpcChannel.ForAddress(
            configuration["Lnd:RpcAddress"] ?? throw new Exception("Lnd rpc address config is missing"),
            new GrpcChannelOptions
            {
                HttpHandler = httpClientHandler,
                Credentials = credentials
            });
        _rpcClient = new Lightning.LightningClient(channel);
    }

    public async Task<string> FetchInvoiceAsync(long valueMillisats, string username, string? comment)
    {
        try
        {
            var invoice = new Invoice
            {
                Memo = comment ?? $"payment for {username}",
                ValueMsat = valueMillisats
            };

            var response = await _rpcClient.AddInvoiceAsync(invoice);
            return response.PaymentRequest;
        }
        catch (Exception e)
        {
            const string errorMessage = "Error fetching invoice from server";
            _logger.LogError(e, errorMessage);
            throw new Exception(errorMessage);
        }
    }

    public async Task<bool> CheckConnection()
    {
        try
        {
            var response = await _rpcClient.GetInfoAsync(new GetInfoRequest());
            return response.SyncedToChain && response.SyncedToGraph;
        }
        catch (Exception e)
        {
            const string errorMessage = "Error fetching server info";
            _logger.LogError(e, errorMessage);
            throw new Exception(errorMessage);
        }
    }

    private Task AddMacaroon(AuthInterceptorContext context, Metadata metadata)
    {
        metadata.Add(new Metadata.Entry("macaroon", _macaroon));
        return Task.CompletedTask;
    }
}