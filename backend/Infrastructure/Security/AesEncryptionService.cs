using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Shared.Security;

namespace Infrastructure.Security;

public sealed class AesEncryptionService : IEncryptionService
{
    private readonly byte[]? _key;

    public AesEncryptionService(IConfiguration configuration)
    {
        var keyB64 = configuration["Encryption:Key"]; // base64 of 32 bytes
        if (!string.IsNullOrWhiteSpace(keyB64))
        {
            try { _key = Convert.FromBase64String(keyB64); }
            catch { _key = null; }
        }
    }

    public string? Encrypt(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext) || _key == null) return plaintext;
        var nonce = RandomNumberGenerator.GetBytes(12);
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[16];
        using var gcm = new AesGcm(_key);
        gcm.Encrypt(nonce, plainBytes, cipher, tag);
        var combined = new byte[1 + nonce.Length + tag.Length + cipher.Length];
        combined[0] = 1; // version
        Buffer.BlockCopy(nonce, 0, combined, 1, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, 1 + nonce.Length, tag.Length);
        Buffer.BlockCopy(cipher, 0, combined, 1 + nonce.Length + tag.Length, cipher.Length);
        return Convert.ToBase64String(combined);
    }

    public string? Decrypt(string? ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext) || _key == null) return ciphertext;
        byte[] combined;
        try { combined = Convert.FromBase64String(ciphertext); }
        catch { return ciphertext; }
        if (combined.Length < 1 + 12 + 16) return ciphertext;
        var version = combined[0];
        if (version != 1) return ciphertext;
        var nonce = new byte[12];
        var tag = new byte[16];
        var cipher = new byte[combined.Length - 1 - nonce.Length - tag.Length];
        Buffer.BlockCopy(combined, 1, nonce, 0, nonce.Length);
        Buffer.BlockCopy(combined, 1 + nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(combined, 1 + nonce.Length + tag.Length, cipher, 0, cipher.Length);
        var plain = new byte[cipher.Length];
        try
        {
            using var gcm = new AesGcm(_key);
            gcm.Decrypt(nonce, cipher, tag, plain);
            return Encoding.UTF8.GetString(plain);
        }
        catch
        {
            return ciphertext;
        }
    }
}






