using System.Text.Json.Serialization;
using EnumStringValues;

namespace IrGateways.Zarinpal;

public enum Currency
{
    [StringValue("IRR")] Rial,
    [StringValue("IRT")] Toman
}

internal record RequestDto(
    [property: JsonPropertyName("merchant_id")] string MerchantId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("callback_url")] string CallbackUrl,
    [property: JsonPropertyName("mobile")] string? Mobile,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("order_id")] string? OrderId
);

public enum FeeType
{
    Merchant,
    Customer //TODO: naming this like api returned value name
}

public record ZarinpalCreateResultDto(short Code, string Message, string Authority, FeeType FeeType, int Fee,
    string Url);

internal record VerifyDto(
    [property: JsonPropertyName("merchant_id")] string MerchantId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("authority")] string? Authority
);

public record ZarinpalVerifyResultDto(short Code,long RefId,string CardPan,string CardHash,FeeType FeeType,int Fee);