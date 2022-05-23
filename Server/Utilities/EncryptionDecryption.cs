using System.Security.Cryptography;
using System.Text;

namespace BlazorAuthenticationLearn.Server.Utilities;

public class EncryptionDecryption
{
    public (byte[], byte[]) EncryptString(string key, byte[] byteArray)
    {
        var rnd = new Random();
        var iv = new byte[16];
        rnd.NextBytes(iv);
        byte[] array;

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor();
        
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            using (var streamWriter = new BinaryWriter(cryptoStream))
            {
                streamWriter.Write(byteArray);
            }

            array = memoryStream.ToArray();
        }
        
        return (array, iv);
    }

    public byte[] DecryptString(string key, byte[] cipherArray, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        
        var decryptor = aes.CreateDecryptor();

        using var memoryStream = new MemoryStream(cipherArray);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new BinaryReader(cryptoStream);
        return streamReader.ReadBytes(cipherArray.Length);
    }
}
