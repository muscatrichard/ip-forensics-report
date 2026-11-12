using System.Text;
using IPForensics.Core.Exceptions;
using IPForensics.Infrastructure.Security;
using Xunit;

namespace IPForensics.Tests;

public class ReportEncryptorTests
{
    private readonly ReportEncryptor _encryptor = TestData.Encryptor();
    private readonly Guid _owner = Guid.CreateVersion7();

    [Fact]
    public void Decrypt_ReturnsTheOriginalReport()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);

        var report = _encryptor.Decrypt(payload, _owner);

        Assert.Equal("8.8.8.8", report.IpAddress);
        Assert.Equal(7, report.Reputation.AbuseConfidenceScore);
        Assert.Equal(188, report.Reputation.TotalReports);
        Assert.Equal(new DateTimeOffset(2026, 11, 10, 18, 26, 57, TimeSpan.Zero), report.Reputation.LastReportedAt);
        Assert.Equal("Ashburn", report.Geolocation.City);
        Assert.True(report.Network.Proxy);
        Assert.True(report.Network.Hosting);
        Assert.False(report.Network.Tor);
    }

    [Fact]
    public void Encrypt_UsesAFreshNonceEveryTime()
    {
        var first = _encryptor.Encrypt(TestData.Report(), _owner);
        var second = _encryptor.Encrypt(TestData.Report(), _owner);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Encrypt_DoesNotLeaveThePlaintextInThePayload()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);

        // Latin1 maps every byte to one character, so any readable text would show up.
        var text = Encoding.Latin1.GetString(payload);

        Assert.DoesNotContain("8.8.8.8", text);
        Assert.DoesNotContain("Ashburn", text);
    }

    [Fact]
    public void Encrypt_StartsWithTheFormatVersion()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);

        Assert.Equal((byte)1, payload[0]);
    }

    [Fact]
    public void Decrypt_FailsForAnotherUser()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);

        Assert.Throws<ReportDecryptionException>(() => _encryptor.Decrypt(payload, Guid.CreateVersion7()));
    }

    [Fact]
    public void Decrypt_FailsWhenTheCiphertextIsChanged()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);
        payload[^1] ^= 0xFF;

        Assert.Throws<ReportDecryptionException>(() => _encryptor.Decrypt(payload, _owner));
    }

    [Fact]
    public void Decrypt_FailsWithADifferentKey()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);
        var otherEncryptor = TestData.Encryptor();

        Assert.Throws<ReportDecryptionException>(() => otherEncryptor.Decrypt(payload, _owner));
    }

    [Fact]
    public void Decrypt_RejectsAnUnsupportedFormatVersion()
    {
        var payload = _encryptor.Encrypt(TestData.Report(), _owner);
        payload[0] = 2;

        Assert.Throws<ReportDecryptionException>(() => _encryptor.Decrypt(payload, _owner));
    }

    [Fact]
    public void Decrypt_RejectsAPayloadThatIsTooShort()
    {
        Assert.Throws<ReportDecryptionException>(() => _encryptor.Decrypt(new byte[5], _owner));
    }
}
