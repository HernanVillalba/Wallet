using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Wallet.Business.Operations
{
    public class PasswordHash
    {
        public static string Generate(string password, byte[] salt = null, bool needsOnlyHash = false)
        {
            //si salt es null es una clave nueva, 
            //sino se pasa el salt para recrear la contraseña y comparar
            if (salt == null || salt.Length != 16)
            {
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (needsOnlyHash) return hashed;

            return $"{hashed}:{ Convert.ToBase64String(salt)}";
        }

        public static bool VerifyPassword(string hashedPasswordWithSalt, string passwordToCheck)
        {
            // separar salt de la contraseña
            var passwordAndHash = hashedPasswordWithSalt.Split(':');
            var salt = Convert.FromBase64String(passwordAndHash[1]);

            // encriptar contraseña a chequear usando el mismo salt que la contraseña guardada
            var hashOfpasswordToCheck = Generate(passwordToCheck, salt, true);

            // comparar ambas contraseñas
            var savedPassword = Convert.FromBase64String(passwordAndHash[0]);
            var hashedNewPasword = Convert.FromBase64String(hashOfpasswordToCheck);
            return SlowEquals(savedPassword, hashedNewPasword);
        }

        //comparación más segura
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}



