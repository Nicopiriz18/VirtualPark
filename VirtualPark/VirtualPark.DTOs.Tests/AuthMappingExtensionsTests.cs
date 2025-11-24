// <copyright file="AuthMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Auth;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class AuthMappingExtensionsTests
{
    [TestMethod]
    public void ToRegisterResponseDto_ShouldMapVisitorToRegisterResponseDto()
    {
        // Arrange
        var visitor = new Visitor(
            "John",
            "Doe",
            "john@example.com",
            "Password123!",
            new DateTime(1990, 5, 15),
            MembershipLevel.Standard,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToRegisterResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitor.Id, dto.Id);
        Assert.AreEqual("John Doe", dto.FullName);
        Assert.AreEqual("john@example.com", dto.Email);
    }

    [TestMethod]
    public void ToRegisterResponseDto_ShouldConcatenateNamesCorrectly()
    {
        // Arrange
        var visitor = new Visitor(
            "Alice",
            "Johnson",
            "alice@example.com",
            "Password123!",
            new DateTime(1985, 8, 10),
            MembershipLevel.VIP,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToRegisterResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Alice Johnson", dto.FullName);
    }
}
