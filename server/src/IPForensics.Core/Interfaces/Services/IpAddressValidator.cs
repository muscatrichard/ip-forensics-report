using System.Net;
using System.Net.Sockets;
using IPForensics.Core.Exceptions;

namespace IPForensics.Core.Services;

public static class IpAddressValidator
{
    public static string ValidatePublic(string? input)
    {
        var text = input?.Trim();

        if (string.IsNullOrEmpty(text))
        {
            throw new InvalidIpAddressException("Enter an IP address.");
        }

        if (!IPAddress.TryParse(text, out var address) || text.Contains('%') || !HasValidShape(text, address))
        {
            throw new InvalidIpAddressException("That is not a valid IPv4 or IPv6 address.");
        }

        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        if (!IsPublic(address))
        {
            throw new InvalidIpAddressException("Only public IP addresses can be looked up.");
        }

        return address.ToString();
    }

    private static bool HasValidShape(string text, IPAddress address)
    {
        if (address.AddressFamily != AddressFamily.InterNetwork)
        {
            return true;
        }

        return text.Split('.').Length == 4 && text.All(c => char.IsAsciiDigit(c) || c == '.');
    }

    private static bool IsPublic(IPAddress address)
    {
        if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.IPv6Any))
        {
            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var b = address.GetAddressBytes();

            return !(b[0] == 0                                    // "this network"
                || b[0] == 10                                     // private
                || (b[0] == 100 && b[1] is >= 64 and <= 127)      // carrier-grade NAT
                || (b[0] == 169 && b[1] == 254)                   // link-local
                || (b[0] == 172 && b[1] is >= 16 and <= 31)       // private
                || (b[0] == 192 && b[1] == 168)                   // private
                || b[0] >= 224);                                  // multicast and reserved
        }

        return !(address.IsIPv6LinkLocal
            || address.IsIPv6SiteLocal
            || address.IsIPv6UniqueLocal
            || address.IsIPv6Multicast);
    }
}
