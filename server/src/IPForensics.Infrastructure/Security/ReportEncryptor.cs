using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IPForensics.Core.Entities;
using IPForensics.Core.Exceptions;
using IPForensics.Core.Interfaces.Security;
using IPForensics.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace IPForensics.Infrastructure.Security;

public sealed class ReportEncryptor : IReportEncryptor
{
    private const byte FormatVersion = 1;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int HeaderSize = 1 + NonceSize + TagSize;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly byte[] _key;

    public ReportEncryptor(IOptions<EncryptionOptions> options)
    {
        _key = Convert.FromBase64String(options.Value.Key);
    }

    public byte[] Encrypt(IPForensicsReport report, Guid userId)
    {
        var plaintext = JsonSerializer.SerializeToUtf8Bytes(report, JsonOptions);

        var payload = new byte[HeaderSize + plaintext.Length];
        payload[0] = FormatVersion;

        var nonce = payload.AsSpan(1, NonceSize);
        var tag = payload.AsSpan(1 + NonceSize, TagSize);
        var ciphertext = payload.AsSpan(HeaderSize);
        
        RandomNumberGenerator.Fill(nonce);

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, AssociatedData(userId));

        return payload;
    }

    public IPForensicsReport Decrypt(byte[] payload, Guid userId)
    {
        if (payload.Length < HeaderSize || payload[0] != FormatVersion)
        {
            throw new ReportDecryptionException("The stored report has an unsupported format.");
        }

        var nonce = payload.AsSpan(1, NonceSize);
        var tag = payload.AsSpan(1 + NonceSize, TagSize);
        var ciphertext = payload.AsSpan(HeaderSize);
        var plaintext = new byte[ciphertext.Length];

        try
        {
            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(nonce, ciphertext, tag, plaintext, AssociatedData(userId));

            return JsonSerializer.Deserialize<IPForensicsReport>(plaintext, JsonOptions)
                ?? throw new ReportDecryptionException("The stored report is empty.");
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException)
        {
            throw new ReportDecryptionException("The stored report could not be decrypted.", ex);
        }
    }

    private static byte[] AssociatedData(Guid userId) => Encoding.UTF8.GetBytes(userId.ToString("D"));
}
