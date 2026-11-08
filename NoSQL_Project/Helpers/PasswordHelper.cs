// Author: Student1
using System.Security.Cryptography;

namespace NoSQL_Project.Helpers
{
    /// <summary>
    /// PBKDF2-SHA256 password hashing helper.
    /// Shared between the web app and the Seeder console app so the same
    /// algorithm is used in both places — no extra NuGet package required.
    ///
    /// Format stored in DB: Base64( byte[0x01] + salt[16] + hash[32] )
    /// The 0x01 version prefix allows future algorithm migration.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize  = 16;
        private const int HashSize  = 32;
        private const int Iterations = 100_000;

        /// <summary>
        /// Hashes <paramref name="password"/> with a random salt using PBKDF2-SHA256.
        /// </summary>
        /// <param name="password">Plaintext password to hash.</param>
        /// <returns>Base64-encoded string safe to store in MongoDB.</returns>
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

            byte[] combined = new byte[1 + SaltSize + HashSize];
            combined[0] = 0x01; // version
            Buffer.BlockCopy(salt, 0, combined, 1,            SaltSize);
            Buffer.BlockCopy(hash, 0, combined, 1 + SaltSize, HashSize);

            return Convert.ToBase64String(combined);
        }

        /// <summary>
        /// Verifies <paramref name="providedPassword"/> against a stored hash.
        /// Uses constant-time comparison to resist timing attacks.
        /// </summary>
        /// <param name="hashedPassword">Stored hash from the database.</param>
        /// <param name="providedPassword">Plaintext candidate password.</param>
        /// <returns><c>true</c> if the password matches; <c>false</c> otherwise.</returns>
        public static bool Verify(string hashedPassword, string providedPassword)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(hashedPassword);
                if (combined.Length != 1 + SaltSize + HashSize || combined[0] != 0x01)
                    return false;

                byte[] salt       = new byte[SaltSize];
                byte[] storedHash = new byte[HashSize];
                Buffer.BlockCopy(combined, 1,            salt,       0, SaltSize);
                Buffer.BlockCopy(combined, 1 + SaltSize, storedHash, 0, HashSize);

                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    providedPassword, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
