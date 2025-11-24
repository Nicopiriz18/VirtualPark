// <copyright file="VisitorTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class VisitorTests
{
    [TestMethod]
    public void CreateVisitor_WithValidData_ShouldSucceed()
    {
        var birthDate = new DateTime(2000, 1, 1);
        var nfcId = Guid.NewGuid();
        var visitor = new Visitor("John", "Doe", "john.doe@example.com", "password123", birthDate, MembershipLevel.Premium, nfcId);
        Assert.AreEqual(birthDate, visitor.BirthDate);
        Assert.AreEqual(MembershipLevel.Premium, visitor.MembershipLevel);
        Assert.AreEqual(nfcId, visitor.NfcId);
        Assert.AreEqual("John", visitor.Name);
        Assert.AreEqual("Doe", visitor.Surname);
        Assert.AreEqual("john.doe@example.com", visitor.Email);
    }

    [TestMethod]
    public void CreateVisitor_WithEmptyName_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Visitor(string.Empty, "Doe", "john.doe@example.com", "password123", new DateTime(2000, 1, 1), MembershipLevel.Standard, Guid.NewGuid()));
    }

    [TestMethod]
    public void CreateVisitor_WithFutureBirthDate_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Visitor("John", "Doe", "john.doe@example.com", "password123", DateTime.UtcNow.AddDays(1), MembershipLevel.Standard, Guid.NewGuid()));
    }

    [TestMethod]
    public void CreateVisitor_WithEmptyNfcId_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Visitor("John", "Doe", "john.doe@example.com", "password123", new DateTime(2000, 1, 1), MembershipLevel.Standard, Guid.Empty));
    }
}
