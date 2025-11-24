// <copyright file="PasswordHasher.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int DefaultIterations = 120_000;
    private const string Scheme = "PBKDF2";
    private const string Version = "V1";
    private const string Prf = "HMACSHA256";

    public string Hash(string plain)
    {
        if (string.IsNullOrWhiteSpace(plain))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(plain));
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = DeriveKey(plain, salt, DefaultIterations, KeySize);

        return $"{Scheme}${Version}$PRF={Prf}$iter={DefaultIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public bool Verify(string plain, string hash)
    {
        if (string.IsNullOrEmpty(plain) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        try
        {
            var parts = hash.Split('$');
            if (parts.Length != 6)
            {
                return false;
            }

            if (!string.Equals(parts[0], Scheme, StringComparison.Ordinal))
            {
                return false;
            }

            var prfPair = parts[2].Split('=');
            if (prfPair.Length != 2 || !string.Equals(prfPair[0], "PRF", StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(prfPair[1], Prf, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var iterPair = parts[3].Split('=');
            if (iterPair.Length != 2 || !string.Equals(iterPair[0], "iter", StringComparison.Ordinal))
            {
                return false;
            }

            if (!int.TryParse(iterPair[1], out var iterations) || iterations <= 0)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[4]);
            var expected = Convert.FromBase64String(parts[5]);

            var actual = DeriveKey(plain, salt, iterations, keySize: expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] DeriveKey(string password, byte[] salt, int iterations, int keySize) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, keySize);
}
