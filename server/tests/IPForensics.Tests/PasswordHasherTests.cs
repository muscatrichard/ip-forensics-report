using IPForensics.Infrastructure.Security;
using Xunit;

namespace IPForensics.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsDifferentValuesForTheSamePassword()
    {
        var first = _hasher.Hash("same-password");
        var second = _hasher.Hash("same-password");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Hash_DoesNotContainThePassword()
    {
        Assert.DoesNotContain("same-password", _hasher.Hash("same-password"));
    }

    [Fact]
    public void Verify_AcceptsTheCorrectPassword()
    {
        var hash = _hasher.Hash("same-password");

        Assert.True(_hasher.Verify("same-password", hash));
    }

    [Fact]
    public void Verify_RejectsAWrongPassword()
    {
        var hash = _hasher.Hash("same-password");

        Assert.False(_hasher.Verify("other-password", hash));
    }

    [Fact]
    public void Verify_AcceptsEveryHashOfTheSamePassword()
    {
        var first = _hasher.Hash("same-password");
        var second = _hasher.Hash("same-password");

        Assert.True(_hasher.Verify("same-password", first));
        Assert.True(_hasher.Verify("same-password", second));
    }
}
