using System.Text.Json.Nodes;
using EnumStringValues;
using RestSharp;

namespace IrGateways.Zarinpal;

public class ZarinpalClient
{
    private readonly string _merchantId;
    
    public ZarinpalClient(string merchantId)
    {
        if (merchantId.Length != 36)
            throw new ArgumentException("Invalid merchant id. merchantId must be 36 character", nameof(merchantId));

        _merchantId = merchantId;
    }

    private const string RequestUrl = "https://api.zarinpal.com/pg/v4/payment/request.json";

    public async Task<ZarinpalCreateResultDto> CreateAsync(
        string callbackUrl,
        ulong amount,
        string description,
        Currency? currency = null,
        string? mobile = null,
        string? email = null,
        string? orderId = null)
    {
        try
        {
            var client = new RestClient(RequestUrl);
            var request = new RestRequest();

            var body = new RequestDto(
                _merchantId,
                amount,
                currency.GetStringValue(),
                description,
                callbackUrl,
                mobile,
                email,
                orderId
            );
            request.AddJsonBody(body);

            var response = await client.PostAsync(request);

            var resultObject = JsonNode.Parse(response.Content);
            var errors = resultObject["errors"];
            if (errors is not JsonArray) //when it's not null and there is an error
            {
                throw new IrGatewaysException(
                    $"gateway create url is failure. Error code is : {errors["code"]}. message is : {errors["message"]} \n" +
                    $"validation errors are : {errors["validations"]}" +
                    " please check the https://www.zarinpal.com/docs/paymentGateway/errorList.html for more information \n ",
                    (short)errors["code"]);
            }

            var data = resultObject["data"];

            return new ZarinpalCreateResultDto(
                (short)data["code"],
                (string)data["message"],
                (string)data["authority"],
                GetFeeTypeAsEnum((string)data["fee_type"]),
                (short)data["fee"],
                Url: $"https://www.zarinpal.com/pg/StartPay/{data["authority"]}"
            );
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, -1);
        }
    }

    private const string VerifyUrl = "https://api.zarinpal.com/pg/v4/payment/verify.json";
    public async Task<ZarinpalVerifyResultDto> VerifyAsync(ulong amount, string authority)
    {
        try
        {
            var client = new RestClient(VerifyUrl);
            var request = new RestRequest();

            var body = new VerifyDto(
                _merchantId,
                amount,
                authority
            );
            request.AddJsonBody(body);

            var response = await client.PostAsync(request);

            var resultObject = JsonNode.Parse(response.Content);
            var errors = resultObject["errors"];
            if (errors is not JsonArray) //when it's not null and there is an error
            {
                throw new IrGatewaysException(
                    $"gateway verify is failure. Error code is : {errors["code"]}. message is : {errors["message"]} \n" +
                    $"validation errors are : {errors["validations"]}" +
                    " please check the https://www.zarinpal.com/docs/paymentGateway/errorList.html for more information \n ",
                    (short)errors["code"]);
            }

            var data = resultObject["data"];

            return new ZarinpalVerifyResultDto(
                (short)data["code"],
                (long)data["red_id"],
                (string)data["card_pan"],
                (string)data["card_hash"],
                GetFeeTypeAsEnum((string)data["fee_type"]),
                (short)data["fee"]
            );
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, -1);
        }
    }

    private static FeeType GetFeeTypeAsEnum(string feeTypeString)
    {
        return feeTypeString == "Merchant" ? FeeType.Merchant : FeeType.Customer;
    }
}