using System.Text.Json;
using System.Text.Json.Nodes;
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

    private const string CreateUrl = "https://api.zarinpal.com/pg/v4/payment/request.json";
    public async Task<ZarinpalCreateResult> CreateAsync(
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
            var client = new RestClient(CreateUrl);
            var request = new RestRequest();
            var body = new CreateDto(
                _merchantId,
                amount,
                currency.ToString(),
                description,
                callbackUrl,
                mobile,
                email,
                orderId
            );
            request.AddJsonBody(body);
            var response = await client.PostAsync(request);

            var resultNode = GetResultNodeFromResponseContent(response.Content);
            CheckHasError(resultNode,"create");

            var data = GetDataNode(resultNode);

            return new ZarinpalCreateResult(
                (short)data["code"],
                (string)data["message"],
                (string)data["authority"],
                GetFeeTypeAsEnum((string)data["fee_type"]),
                (short)data["fee"],
                RedirectUrl: $"https://www.zarinpal.com/pg/StartPay/{data["authority"]}"
            );
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, -1);
        }
    }

    private const string VerifyUrl = "https://api.zarinpal.com/pg/v4/payment/verify.json";
    public async Task<ZarinpalVerifyResult> VerifyAsync(ulong amount, string authority)
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

            var resultNode = GetResultNodeFromResponseContent(response.Content);
            CheckHasError(resultNode,"verify");
            
            var data = GetDataNode(resultNode);

            return new ZarinpalVerifyResult(
                (short)data["code"],
                (long)data["ref_id"],
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

    private const string UnverifiedUrl = "https://api.zarinpal.com/pg/v4/payment/unVerified.json";
    public async Task<List<UnVerifiedAuthority>> GetUnverifiedAuthoritiesAsync()
    {
        try
        {
            var client = new RestClient(UnverifiedUrl);
            var request = new RestRequest();
            var body = new UnVerifiedDto(_merchantId);
            request.AddJsonBody(body);
            var response = await client.PostAsync(request);

            var resultNode = GetResultNodeFromResponseContent(response.Content);
            CheckHasError(resultNode,"unverified");

            var data = GetDataNode(resultNode);

            return JsonSerializer.Deserialize<List<UnVerifiedAuthority>>(data["authorities"]);
        }
        catch (Exception e)
        {
            throw new IrGatewaysException(e.Message, 1);
        }
    }
    
    private static FeeType GetFeeTypeAsEnum(string feeTypeString)
    {
        return feeTypeString == "Merchant" ? FeeType.Merchant : FeeType.Payer;
    }

    /// <summary>
    /// to check if there is error from api result and throw exception
    /// </summary>
    /// <param name="resultNode">api response to get error results and check it</param>
    /// <param name="methodName">that method cuz this error happening to show request method name in the exception message</param>
    /// <exception cref="IrGatewaysException"></exception>
    private static void CheckHasError(JsonNode resultNode,string methodName)
    {
        var errorsNode = GetErrorsNode(resultNode);
        if (errorsNode is not JsonArray) //when it's not null and there is an error
        {
            throw new IrGatewaysException(
                $"zarinpal gateway {methodName} is failure. Error code is : {errorsNode["code"]}. message is : {errorsNode["message"]} \n" +
                $"validation errors are : {errorsNode["validations"]}" +
                " please check the https://www.zarinpal.com/docs/paymentGateway/errorList.html for more information \n ",
                (short)errorsNode["code"]);
        }
    }

    private static JsonNode GetResultNodeFromResponseContent(string responseContent)
        => JsonNode.Parse(responseContent);
    private static JsonNode GetErrorsNode(JsonNode resultNode)
        => resultNode["errors"];

    private static JsonNode GetDataNode(JsonNode resultNode)
        => resultNode["data"];
}