namespace IrGateways;

public class IrGatewaysException : Exception
{
    public IrGatewaysException(string message, short code) : base(message)
    {
        this.Code = code;
    }

    public short Code { get; }
}