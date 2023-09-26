namespace IrGateways.Nextpay;

public static class NextpayClientExtensions
{
    public static string ToStringValue(this AutoVerify autoVerify)
    {
        return autoVerify switch
        {
            AutoVerify.Yes => "yes",
            AutoVerify.No => "no"
        };
    }
}