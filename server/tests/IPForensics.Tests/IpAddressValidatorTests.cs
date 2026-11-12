using IPForensics.Core.Exceptions;
using IPForensics.Core.Services;
using Xunit;

namespace IPForensics.Tests;

public class IpAddressValidatorTests
{
    [Theory]
    [InlineData("8.8.8.8", "8.8.8.8")]
    [InlineData("  8.8.8.8  ", "8.8.8.8")]
    [InlineData("2001:4860:4860::8888", "2001:4860:4860::8888")]
    [InlineData("2001:4860:4860:0:0:0:0:8888", "2001:4860:4860::8888")]
    [InlineData("2606:4700:4700::1111", "2606:4700:4700::1111")]
    [InlineData("::ffff:8.8.8.8", "8.8.8.8")]
    public void ValidatePublic_ReturnsTheCanonicalAddress(string input, string expected)
    {
        Assert.Equal(expected, IpAddressValidator.ValidatePublic(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("999.1.1.1")]
    [InlineData("1")]
    [InlineData("8.8")]
    [InlineData("8.8.8")]
    [InlineData("8.8.8.8.8")]
    [InlineData("8.8.8.8 extra")]
    [InlineData("0x8.0x8.0x8.0x8")]
    [InlineData("2001:4860:4860::zzzz")]
    [InlineData("2001:4860:4860::8888%eth0")]
    public void ValidatePublic_RejectsMalformedInput(string? input)
    {
        Assert.Throws<InvalidIpAddressException>(() => IpAddressValidator.ValidatePublic(input));
    }

    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("10.0.0.1")]
    [InlineData("100.64.0.1")]
    [InlineData("127.0.0.1")]
    [InlineData("169.254.1.1")]
    [InlineData("172.16.0.1")]
    [InlineData("192.168.1.1")]
    [InlineData("224.0.0.1")]
    [InlineData("255.255.255.255")]
    [InlineData("::")]
    [InlineData("::1")]
    [InlineData("fe80::1")]
    [InlineData("fd00::1")]
    [InlineData("ff02::1")]
    [InlineData("::ffff:10.0.0.1")]
    public void ValidatePublic_RejectsNonPublicAddresses(string input)
    {
        var exception = Assert.Throws<InvalidIpAddressException>(() => IpAddressValidator.ValidatePublic(input));

        Assert.Contains("public", exception.Message);
    }
}
