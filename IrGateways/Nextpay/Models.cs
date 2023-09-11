using System.Text.Json.Serialization;

namespace IrGateways.Nextpay;

public enum AutoVerify
{
    yes,
    no
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
