using System.Text.Json.Serialization;
using IrGateways.Assets;

namespace IrGateways.Zarinpal;

public enum Currency
{
    IRR,
    IRT
}

internal record CreateDto(
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
    Payer
}

public record ZarinpalCreateResult(short Code, string Message, string Authority, FeeType FeeType, int Fee,
    string RedirectUrl);

internal record VerifyDto(
    [property: JsonPropertyName("merchant_id")] string MerchantId,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("authority")] string? Authority
);

public record ZarinpalVerifyResult(short Code,long RefId,string CardPan,string CardHash,FeeType FeeType,int Fee);

internal record UnVerifiedDto(
    [property: JsonPropertyName("merchant_id")] string MerchantId
);
public record UnVerifiedAuthority(
    [property: JsonPropertyName("authority")] string Authority,
    [property: JsonPropertyName("amount")] ulong Amount,
    [property: JsonPropertyName("callback_url")] string CallbackUrl,
    [property: JsonPropertyName("referer")]string Referer,
    [property: JsonPropertyName("date"),JsonConverter(typeof(IsoTimeJsonConverter))] DateTime Date
);