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
                autoVerify?.ToStringValue(),
                allowedCard
            );
            request.AddJsonBody(body);
            var result = await client.PostAsync<CreateResponseDto>(request);
            
            if (!IsCreateSuccessful(result.Code))
                throw new IrGatewaysException(
                    $"nextpay gateway create is failure. Error code is : {result.Code} " +
                    $" please visit the https://nextpay.org/nx/docs#step-7 for more information ",
                    result.Code
                );

            return new NextpayCreateResult(result.Code, result.TransId,
                $"https://nextpay.org/nx/gateway/payment/{result.TransId}");
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, 1);
        }
    }

    public async Task<NextpayVerifyResult> VerifyAsync(string transId, ulong amount, Currency? currency = null)
    {
        try
        {
            var client = new RestClient(CreateUrl);
            var request = new RestRequest();
            var body = new VerifyDto(
                _apiKey,
                transId,
                amount,
                currency.ToString()
            );
            request.AddJsonBody(body);
            var result = await client.PostAsync<NextpayVerifyResult>(request);

            if (!IsVerifySuccessful(result.Code))
                throw new IrGatewaysException(
                    $"nextpay gateway verify is failure. Error code is : {result.Code} " +
                    $" please visit the https://nextpay.org/nx/docs#step-7 for more information ",
                    result.Code
                );

            return result;
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, 1);
        }
    }

    public async Task<NextpayRefundResult> RefundAsync(string transId,ulong amount)
    {
        try
        {
            var client = new RestClient(CreateUrl);
            var request = new RestRequest();
            var body = new RefundDto(
                _apiKey,
                transId,
                amount
            );
            request.AddJsonBody(body);
            var result = await client.PostAsync<NextpayRefundResult>(request);
            
            if (!IsRefundSuccessful(result.Code))
                throw new IrGatewaysException(
                    $"nextpay gateway refund is failure. Error code is : {result.Code} " +
                    $" please visit the https://nextpay.org/nx/docs#step-7 for more information ",
                    result.Code
                );
            
            return result;
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, 1);
        }
    }

    private static bool IsCreateSuccessful(short resultCode)
        => resultCode is -1;
    
    private static bool IsVerifySuccessful(short resultCode)
        => resultCode is 0;

    private static bool IsRefundSuccessful(short resultCode)
        => resultCode is -90;
}