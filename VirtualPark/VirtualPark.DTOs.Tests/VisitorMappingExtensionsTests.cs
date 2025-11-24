// <copyright file="VisitorMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Visitors;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class VisitorMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapVisitorToDtoCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(1990, 5, 15);
        var nfcId = Guid.NewGuid();
        var visitor = new Visitor(
            "John",
            "Doe",
            "john@example.com",
            "Password123!",
            birthDate,
            MembershipLevel.Standard,
            nfcId);

        // Act
        var dto = visitor.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitor.Id, dto.Id);
        Assert.AreEqual("John", dto.Name);
        Assert.AreEqual("Doe", dto.Surname);
        Assert.AreEqual("john@example.com", dto.Email);
        Assert.AreEqual(birthDate, dto.BirthDate);
        Assert.AreEqual(MembershipLevel.Standard, dto.MembershipLevel);
        Assert.AreEqual(nfcId, dto.NfcId);
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapVisitorToDetailDto()
    {
        // Arrange
        var birthDate = new DateTime(1995, 3, 20);
        var visitor = new Visitor(
            "Jane",
            "Smith",
            "jane@example.com",
            "Password123!",
            birthDate,
            MembershipLevel.Premium,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToDetailDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitor.Id, dto.Id);
        Assert.AreEqual("Jane", dto.Name);
        Assert.AreEqual("Smith", dto.Surname);
        Assert.AreEqual(0, dto.TotalTickets);
    }

    [TestMethod]
    public void ToProfileDto_ShouldMapVisitorToProfileDtoWithCalculatedAge()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var visitor = new Visitor(
            "Bob",
            "Johnson",
            "bob@example.com",
            "Password123!",
            birthDate,
            MembershipLevel.VIP,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToProfileDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitor.Id, dto.Id);
        Assert.AreEqual("Bob", dto.Name);
        Assert.AreEqual("Johnson", dto.Surname);
        Assert.AreEqual("Bob Johnson", dto.FullName);
        Assert.AreEqual(birthDate, dto.BirthDate);
        Assert.IsTrue(dto.Age >= 34); // Age calculation
    }

    [TestMethod]
    public void ToProfileDto_ShouldCalculateAgeCorrectly()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var birthDate = today.AddYears(-25).AddDays(-1);
        var visitor = new Visitor(
            "Alice",
            "Williams",
            "alice@example.com",
            "Password123!",
            birthDate,
            MembershipLevel.Standard,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToProfileDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(25, dto.Age);
    }

    [TestMethod]
    public void ToProfileDto_ShouldDecreaseAge_WhenBirthdayNotYetOccurred()
    {
        // Arrange
        var birthDate = new DateTime(DateTime.UtcNow.Year - 30, 12, 31); // Cumple en diciembre (todavía no llegó)
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "Pass123!", birthDate, MembershipLevel.Standard, Guid.NewGuid());

        // Act
        var dto = visitor.ToProfileDto();

        // Assert
        Assert.AreEqual(29, dto.Age);
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfVisitorsToListOfDtos()
    {
        // Arrange
        var visitors = new List<Visitor>
        {
            new Visitor("Visitor1", "Last1", "v1@example.com", "Pass123!", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid()),
            new Visitor("Visitor2", "Last2", "v2@example.com", "Pass123!", new DateTime(1995, 5, 5), MembershipLevel.Premium, Guid.NewGuid()),
        };

        // Act
        var dtos = visitors.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Visitor1", dtos[0].Name);
        Assert.AreEqual("Visitor2", dtos[1].Name);
    }

    [TestMethod]
    public void ToDetailDto_Collection_ShouldMapListOfVisitorsToListOfDetailDtos()
    {
        // Arrange
        var visitors = new List<Visitor>
        {
            new Visitor("Visitor1", "Last1", "v1@example.com", "Pass123!", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid()),
            new Visitor("Visitor2", "Last2", "v2@example.com", "Pass123!", new DateTime(1995, 5, 5), MembershipLevel.Premium, Guid.NewGuid()),
            new Visitor("Visitor3", "Last3", "v3@example.com", "Pass123!", new DateTime(2000, 10, 10), MembershipLevel.VIP, Guid.NewGuid()),
        };

        // Act
        var dtos = visitors.ToDetailDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(3, dtos.Count);
        Assert.AreEqual("Visitor1", dtos[0].Name);
        Assert.AreEqual("Visitor2", dtos[1].Name);
        Assert.AreEqual("Visitor3", dtos[2].Name);
    }
}
