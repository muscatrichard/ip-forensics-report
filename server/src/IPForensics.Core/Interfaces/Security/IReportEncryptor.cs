using IPForensics.Core.Entities;

namespace IPForensics.Core.Interfaces.Security;

public interface IReportEncryptor
{
    byte[] Encrypt(IPForensicsReport report, Guid userId);
    IPForensicsReport Decrypt(byte[] payload, Guid userId);
}
