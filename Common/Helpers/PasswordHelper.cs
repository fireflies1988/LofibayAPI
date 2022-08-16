using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public class PasswordHelper
    {
        public static byte[] GetSecureSalt()
        {
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            return RandomNumberGenerator.GetBytes(128 / 8);
        }

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        public static string HashUsingPbkdf2(string password, byte[] salt)
        {
            byte[] derivedKey = KeyDerivation.Pbkdf2(
                password: password, 
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(derivedKey);
        }
    }
}
