namespace IPForensics.Core.Exceptions;

public sealed class ReportDecryptionException : Exception
{
    public ReportDecryptionException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
