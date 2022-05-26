using System.Security.Cryptography;
using System.Text;

namespace BlazorAuthenticationLearn.Server.Utilities;

public static class EncryptionDecryption
{
    public static byte[] Encrypt(string key, Stream sourceStream, Stream destStream)
    {
        var rnd = new Random();
        var iv = new byte[16];
        rnd.NextBytes(iv);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor();
        using var cryptoStream = new CryptoStream(destStream, encryptor, CryptoStreamMode.Write);
        sourceStream.CopyTo(cryptoStream);
        
        return iv;
    }

    public static void Decrypt(string key, byte[] iv, Stream sourceStream, Stream destStream)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor();
        using var cryptoStream = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read);
        cryptoStream.CopyTo(destStream);
    }
}
