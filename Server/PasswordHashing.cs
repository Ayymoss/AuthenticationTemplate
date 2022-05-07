using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BlazorAuthenticationLearn.Server;

public static class PasswordHashing
{
    public static (string, byte[]) HashPassword(string password, byte[] salt = null)
    {
        if (salt == null)
        {
            salt = new byte[128 / 8];
            RandomNumberGenerator.Create().GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        
        return (hashed, salt);
    }
}
