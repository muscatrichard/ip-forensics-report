namespace IPForensics.Core.Exceptions;

public sealed class InvalidIpAddressException : Exception
{
    public InvalidIpAddressException(string message)
        : base(message)
    {
    }
}
