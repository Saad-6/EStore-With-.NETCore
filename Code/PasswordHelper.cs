namespace EStore.Code;

using System;
using System.Security.Cryptography;
using System.Text;

public  class PasswordHelper
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 10000; // Number of iterations

    /// <summary>
    /// Hashes the password with a randomly generated salt.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A hashed password in the format: {salt}.{hash}</returns>
    public static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(KeySize);

        // Return the hash and salt as a single string (Base64 encoded)
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
    public static string GenerateRandomId(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Verifies if the provided password matches the hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hashedPassword">The hashed password (in format {salt}.{hash}).</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 2);
        if (parts.Length != 2)
            throw new FormatException("Invalid hashed password format.");

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] testHash = pbkdf2.GetBytes(KeySize);

        // Compare byte arrays securely
        return CryptographicOperations.FixedTimeEquals(hash, testHash);
    }
}

