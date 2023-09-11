using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace IrGateways.Nextpay;

public class NextpayClient
{
    private readonly string _apiKey;

    public NextpayClient(string apiKey) //TODO: check apiKey must be only 32 chars or not
    {
        _apiKey = apiKey;
    }

    private const string CreateUrl = "https://nextpay.org/nx/gateway/token";

    public async Task<NextpayCreateResult> CreateAsync(
        string orderId,
        ulong amount,
        string callbackUrl,
        Currency? currency = null,
        string? customerPhone = null,
        string? payerName = null,
        string? description = null,
        AutoVerify? autoVerify = null,
        string? allowedCard = null,
        object? customData = null
    )
    {
        try
        {
            var client = new RestClient(CreateUrl);
            var request = new RestRequest();
            var body = new CreateDto(
                _apiKey,
                orderId,
                amount,
                callbackUrl,
                currency.ToString(),
                customerPhone,
                customData,
                payerName,
                description,
                autoVerify.ToString(),
                allowedCard
            );
            request.AddJsonBody(body);
            var result = await client.PostAsync<CreateResponseDto>(request);
            if (result.Code is not 0)
            {
                throw new IrGatewaysException($"nextpay gateway create is failure. Error code is : {result.Code} " +
                                              $" please visit the https://nextpay.org/nx/docs#step-7 for more information ",
                    result.Code);
            }

            return new NextpayCreateResult(result.Code, result.TransId,
                $"https://nextpay.org/nx/gateway/payment/{result.TransId}");
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, 1);
        }
    }
}