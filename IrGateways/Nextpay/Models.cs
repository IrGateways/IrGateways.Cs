using System.Text.Json.Serialization;

namespace IrGateways.Nextpay;

public enum AutoVerify
{
    Yes,
    No
}

public enum Currency
{
    IRR,
    IRT
}

internal record CreateDto(
    [property: JsonPropertyName("api_key")] string ApiKey,
    [property: JsonPropertyName("order_id")] string OrderId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("callback_url")] string CallbackUrl,
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("customer_phone")] string? CustomerPhone,
    [property: JsonPropertyName("custom_json_fields")] object? CustomJsonFields,
    [property: JsonPropertyName("payer_name")] string? PayerName,
    [property: JsonPropertyName("payer_desc")] string? PayerDesc,
    [property: JsonPropertyName("auto_verify")] string? AutoVerify, 
    [property: JsonPropertyName("allowed_card")] string? AllowedCard
);

internal record CreateResponseDto(
    [property: JsonPropertyName("code")] short Code,
    [property: JsonPropertyName("trans_id")] string TransId
);
public record NextpayCreateResult(short Code,string TransId,string RedirectUrl);

internal record VerifyDto(
    [property: JsonPropertyName("api_key")] string ApiKey,
    [property: JsonPropertyName("trans_id")] string TransId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("currency")] string? Currency
);
public record NextpayVerifyResult(
    [property: JsonPropertyName("code")] short Code,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("order_id")] string OrderId
);
internal record RefundDto(
    [property: JsonPropertyName("api_key")] string ApiKey,
    [property: JsonPropertyName("trans_id")] string TransId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("refund_request")] string Currency = "yes_money_back"
);
public record NextpayRefundResult(
    [property: JsonPropertyName("code")] short Code,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("order_id")] string OrderId,
    [property: JsonPropertyName("card_holder")] string CardHolder,
    [property: JsonPropertyName("customer_phone")] ulong? CustomerPhone,
    [property: JsonPropertyName("custom")] object? CustomJsonFields
);