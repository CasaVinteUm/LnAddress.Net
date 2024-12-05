using LNUnit.LND;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NLightning.Common.Managers;
using Invoice = NLightning.Bolts.BOLT11.Invoice;
using Network = NLightning.Common.Types.Network;

namespace LnAddress.Net.Tests.IntegrationTests;

using Fixtures;
using LnAddress.Net.Services;

[Collection("regtest")]
public class LndGrpcIntegrationTest
{
    private readonly LightningRegtestFixture _lightningRegtestFixture;
    private readonly LNDNodeConnection _alice;
    private readonly LndService _lndService;

    public LndGrpcIntegrationTest(LightningRegtestFixture fixture)
    {
        _lightningRegtestFixture = fixture;

        var loggerMock = new Mock<ILogger<LndService>>();

        // Configure NLightning.Bolt11 decoder
        ConfigManager.Instance.Network = Network.REG_TEST;

        _alice = _lightningRegtestFixture.Builder?.LNDNodePool?.ReadyNodes.First(x => x.LocalAlias == "alice") ?? throw new Exception("Alice was not ready.");

        var inMemorySettings = new Dictionary<string, string?>{
            { "Lnd:Macaroon", _alice.Settings.MacaroonBase64 },
            { "Lnd:Cert", _alice.Settings.TLSCertBase64 },
            { "Lnd:RpcAddress", _alice.Host }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _lndService = new LndService(configuration, loggerMock.Object);
    }

    [Fact]
    public async Task Given_ValidAmount_When_FetchInvoice_Then_Expect_ValidInvoice()
    {
        // Arrange
        const long expectedAmount = 10_000L;
        const string expectedDescription = "payment for nGoline";
        var expectedPubkey = _alice.LocalNodePubKeyBytes;

        // Act
        var invoice = await _lndService.FetchInvoiceAsync(expectedAmount, "nGoline", null);

        // Assert
        var decodedInvoice = Invoice.Decode(invoice);
        Assert.NotNull(decodedInvoice);
        Assert.Equal(expectedPubkey, decodedInvoice.PayeePubKey?.ToBytes());
        Assert.Equal(expectedAmount, (long)decodedInvoice.AmountMilliSats);
        Assert.Equal(expectedDescription, decodedInvoice.Description);
    }

    [Fact]
    public async Task Given_ValidAmountAndDescription_When_FetchInvoice_Then_Expect_ValidInvoice()
    {
        // Arrange
        const long expectedAmount = 10_000L;
        const string expectedDescription = "LnAddress Payment";
        var expectedPubkey = _alice.LocalNodePubKeyBytes;

        // Act
        var invoice = await _lndService.FetchInvoiceAsync(expectedAmount, "nGoline", expectedDescription);

        // Assert
        var decodedInvoice = Invoice.Decode(invoice);
        Assert.NotNull(decodedInvoice);
        Assert.Equal(expectedPubkey, decodedInvoice.PayeePubKey?.ToBytes());
        Assert.Equal(expectedAmount, (long)decodedInvoice.AmountMilliSats);
        Assert.Equal(expectedDescription, decodedInvoice.Description);
    }

    [Fact]
    public async Task Given_ValidInvoice_When_Paying_Then_NoError()
    {
        // Arrange
        var invoice = await _lndService.FetchInvoiceAsync(10_000, "nGoline", null);
        var carol = _lightningRegtestFixture.Builder?.LNDNodePool?.ReadyNodes.First(x => x.LocalAlias == "carol") ?? throw new Exception("Bob was not ready.");
        // Wait for a second so channels are synced
        await Task.Delay(1_000);

        // Act
        var paymentResponse = await carol.LightningClient.SendPaymentSyncAsync(new()
        {
            PaymentRequest = invoice,
        });

        // Assert
        Assert.NotNull(paymentResponse);
        Assert.Empty(paymentResponse.PaymentError);
        Assert.NotNull(paymentResponse.PaymentPreimage);
    }
}