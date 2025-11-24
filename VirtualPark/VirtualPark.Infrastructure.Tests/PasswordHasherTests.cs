// <copyright file="PasswordHasherTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using VirtualPark.Infrastructure.Security;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class PasswordHasher_AdditionalTests
{
    private PasswordHasher hasher = null!;

    [TestInitialize]
    public void SetUp()
    {
        this.hasher = new PasswordHasher();
    }

    [TestMethod]
    public void Hash_Throws_OnNull()
    {
        Assert.ThrowsException<ArgumentException>(() => this.hasher.Hash(null!));
    }

    [TestMethod]
    public void Hash_Throws_OnEmpty()
    {
        Assert.ThrowsException<ArgumentException>(() => this.hasher.Hash(string.Empty));
    }

    [TestMethod]
    public void Hash_Throws_OnWhitespace()
    {
        Assert.ThrowsException<ArgumentException>(() => this.hasher.Hash("   "));
    }

    [TestMethod]
    public void Hash_SamePassword_ProducesDifferentSaltsAndHashes()
    {
        var p = "MismaClave#123";
        var h1 = this.hasher.Hash(p);
        var h2 = this.hasher.Hash(p);

        Assert.AreNotEqual(h1, h2, "Mismas claves no deben producir el mismo hash (sal aleatoria).");

        var s1 = h1.Split('$')[4];
        var s2 = h2.Split('$')[4];
        Assert.AreNotEqual(s1, s2, "Las sales deben diferir.");

        Assert.IsTrue(this.hasher.Verify(p, h1));
        Assert.IsTrue(this.hasher.Verify(p, h2));
    }

    [TestMethod]
    public void Hash_Embeds_DefaultIterations_Value()
    {
        var h = this.hasher.Hash("Clave!Iter");
        var parts = h.Split('$');
        var iterPair = parts[3].Split('=');

        Assert.AreEqual("iter", iterPair[0]);
        Assert.AreEqual("120000", iterPair[1]);
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenTooManyParts()
    {
        var h = this.hasher.Hash("clave");
        var withExtra = h + "$EXTRA";
        Assert.IsFalse(this.hasher.Verify("clave", withExtra));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenPrfLabelIsWrong()
    {
        var h = this.hasher.Hash("clave");
        var tampered = ReplacePart(h, 2, "ALG=HMACSHA256"); // etiqueta incorrecta
        Assert.IsFalse(this.hasher.Verify("clave", tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenPrfMissingEquals()
    {
        var h = this.hasher.Hash("clave");
        var tampered = ReplacePart(h, 2, "PRFHMACSHA256");
        Assert.IsFalse(this.hasher.Verify("clave", tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenIterationsNotNumber()
    {
        var h = this.hasher.Hash("clave");
        var tampered = ReplacePart(h, 3, "iter=no-num");
        Assert.IsFalse(this.hasher.Verify("clave", tampered));
    }

    [DataTestMethod]
    [DataRow("iter=0")]
    [DataRow("iter=-5")]
    [DataRow("iter=999999999999999999999")]
    public void Verify_ReturnsFalse_WhenIterationsZeroNegativeOrOverflow(string iterValue)
    {
        var h = this.hasher.Hash("clave");
        var tampered = ReplacePart(h, 3, iterValue);
        Assert.IsFalse(this.hasher.Verify("clave", tampered));
    }

    [TestMethod]
    public void Verify_Succeeds_WithPRF_CaseInsensitive()
    {
        var p = "Aa1!bB2@";
        var h = this.hasher.Hash(p);
        var okCase = ReplacePart(h, 2, "PRF=hMaCsHa256");
        Assert.IsTrue(this.hasher.Verify(p, okCase));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenSchemeCaseDiffers()
    {
        var p = "Case#Scheme";
        var h = this.hasher.Hash(p);
        var lower = ReplacePart(h, 0, "pbkdf2");
        Assert.IsFalse(this.hasher.Verify(p, lower));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenSaltIsDifferentButValidBase64()
    {
        var p = "SaltTamper!";
        var h = this.hasher.Hash(p);

        var parts = h.Split('$');
        var saltLen = Convert.FromBase64String(parts[4]).Length;
        var newSalt = RandomNumberGenerator.GetBytes(saltLen);
        var tampered = ReplacePart(h, 4, Convert.ToBase64String(newSalt));

        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenKeyIsDifferentButValidBase64()
    {
        var p = "KeyTamper!";
        var h = this.hasher.Hash(p);

        var parts = h.Split('$');
        var keyLen = Convert.FromBase64String(parts[5]).Length;
        var newKey = RandomNumberGenerator.GetBytes(keyLen);
        var tampered = ReplacePart(h, 5, Convert.ToBase64String(newKey));

        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_Succeeds_EvenIfVersionDiffers_BackwardCompat()
    {
        var p = "VersionOk#";
        var h = this.hasher.Hash(p);
        var otherVersion = ReplacePart(h, 1, "V9");
        Assert.IsTrue(this.hasher.Verify(p, otherVersion));
    }

    [TestMethod]
    public void Hash_FormatAndLengths_AreCorrect()
    {
        var h = this.hasher.Hash("A1a!B2b@");
        var parts = h.Split('$');
        Assert.AreEqual(6, parts.Length);
        Assert.AreEqual("PBKDF2", parts[0]);
        Assert.AreEqual("V1", parts[1]);

        var prf = parts[2].Split('=');
        Assert.AreEqual("PRF", prf[0]);
        Assert.IsTrue(string.Equals(prf[1], "HMACSHA256", StringComparison.OrdinalIgnoreCase));

        var iter = parts[3].Split('=');
        Assert.AreEqual("iter", iter[0]);
        Assert.IsTrue(int.TryParse(iter[1], out var iters) && iters > 0);

        var saltBytes = Convert.FromBase64String(parts[4]);
        var keyBytes = Convert.FromBase64String(parts[5]);
        Assert.AreEqual(16, saltBytes.Length); // SaltSize
        Assert.AreEqual(32, keyBytes.Length); // KeySize
    }

    [TestMethod]
    public void Verify_True_WithManualHash_AndCustomIterations()
    {
        var p = "Custom#Iter1";
        var salt = RandomNumberGenerator.GetBytes(16);
        var iterations = 10_001;
        var key = Rfc2898DeriveBytes.Pbkdf2(p, salt, iterations, HashAlgorithmName.SHA256, 32);

        var manual =
            $"PBKDF2$V1$PRF=HMACSHA256$iter={iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        Assert.IsTrue(this.hasher.Verify(p, manual));
    }

    [TestMethod]
    public void Verify_Succeeds_WithDifferentKeySize_FromHash()
    {
        var p = "KeySize#1";
        var salt = RandomNumberGenerator.GetBytes(16);
        var iterations = 2_000;
        var key16 = Rfc2898DeriveBytes.Pbkdf2(p, salt, iterations, HashAlgorithmName.SHA256, 16);

        var manual =
            $"PBKDF2$V1$PRF=HMACSHA256$iter={iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key16)}";
        Assert.IsTrue(this.hasher.Verify(p, manual)); // usa el tamaño embebido (16)
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnTooFewParts()
    {
        var p = "FewParts#1";
        var h = this.hasher.Hash(p);
        var shortHash = string.Join('$', h.Split('$').Take(5)); // falta la clave
        Assert.IsFalse(this.hasher.Verify(p, shortHash));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnWrongPrfValue()
    {
        var p = "WrongPrf#1";
        var h = this.hasher.Hash(p);
        var tampered = ReplacePart(h, 2, "PRF=HMACSHA1");
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnIterationsLabelWrongCase()
    {
        var p = "IterLabel#1";
        var h = this.hasher.Hash(p);
        var value = h.Split('$')[3].Split('=')[1];
        var tampered = ReplacePart(h, 3, $"ITER={value}"); // etiqueta incorrecta
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnCorruptBase64_Salt()
    {
        var p = "BadSalt#1";
        var h = this.hasher.Hash(p);
        var tampered = ReplacePart(h, 4, "###NOPE###"); // base64 inválido
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnCorruptBase64_Key()
    {
        var p = "BadKey#1";
        var h = this.hasher.Hash(p);
        var tampered = ReplacePart(h, 5, "!!!"); // base64 inválido
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_Succeeds_EvenIfVersionEmpty()
    {
        var p = "EmptyVersion#1";
        var h = this.hasher.Hash(p);
        var tampered = ReplacePart(h, 1, " "); // versión vacía
        Assert.IsTrue(this.hasher.Verify(p, tampered)); // versión se ignora
    }

    [TestMethod]
    public void Verify_ReturnsFalse_OnDifferentPassword()
    {
        var h = this.hasher.Hash("Pa$$w0rd!");
        Assert.IsFalse(this.hasher.Verify("NotTheSame", h));
    }

    [TestMethod]
    public void Hash_DifferentPasswords_ProduceDifferentHashes()
    {
        var h1 = this.hasher.Hash("Alpha#1");
        var h2 = this.hasher.Hash("Beta#2");
        Assert.AreNotEqual(h1, h2);
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenSaltLengthDiffers()
    {
        var p = "LenSalt#1";
        var h = this.hasher.Hash(p);
        var newSalt = RandomNumberGenerator.GetBytes(8); // longitud distinta
        var tampered = ReplacePart(h, 4, Convert.ToBase64String(newSalt));
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_WhenSchemeIsDifferent()
    {
        var p = "Scheme#1";
        var h = this.hasher.Hash(p);
        var tampered = ReplacePart(h, 0, "SCRYPT");
        Assert.IsFalse(this.hasher.Verify(p, tampered));
    }

    private static string ReplacePart(string hash, int index, string newValue)
    {
        var parts = hash.Split('$');
        parts[index] = newValue;
        return string.Join('$', parts);
    }
}
