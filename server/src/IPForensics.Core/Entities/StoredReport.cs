namespace IPForensics.Core.Entities;

public sealed class StoredReport
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAtUtc { get; set; }
}
