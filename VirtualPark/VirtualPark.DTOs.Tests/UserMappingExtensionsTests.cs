// <copyright file="UserMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Users;
using VirtualPark.DTOs.Users.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class UserMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapUserToDtoCorrectly()
    {
        // Arrange
        var user = new User("John", "Doe", "john@example.com", "Password123!", RoleEnum.Administrator);

        // Act
        var dto = user.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(user.Id, dto.Id);
        Assert.AreEqual("John", dto.Name);
        Assert.AreEqual("Doe", dto.Surname);
        Assert.AreEqual("john@example.com", dto.Email);
        Assert.AreEqual(1, dto.Roles.Count);
        Assert.IsTrue(dto.Roles.Contains(RoleEnum.Administrator));
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapUserToDetailDtoCorrectly()
    {
        // Arrange
        var user = new User("Jane", "Smith", "jane@example.com", "Password123!", RoleEnum.Administrator);

        // Act
        var dto = user.ToDetailDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(user.Id, dto.Id);
        Assert.AreEqual("Jane", dto.Name);
        Assert.AreEqual("Smith", dto.Surname);
        Assert.AreEqual("jane@example.com", dto.Email);
        Assert.AreEqual("Jane Smith", dto.FullName);
        Assert.AreEqual(1, dto.Roles.Count);
    }

    [TestMethod]
    public void ToListDto_ShouldMapUserToListDtoCorrectly()
    {
        // Arrange
        var user = new User("Bob", "Johnson", "bob@example.com", "Password123!", RoleEnum.Operator);

        // Act
        var dto = user.ToListDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(user.Id, dto.Id);
        Assert.AreEqual("Bob", dto.Name);
        Assert.AreEqual("Johnson", dto.Surname);
        Assert.AreEqual("bob@example.com", dto.Email);
        Assert.AreEqual(1, dto.Roles.Count);
    }

    [TestMethod]
    public void ToDomain_ShouldMapCreateUserRequestDtoToUser()
    {
        // Arrange
        var dto = new CreateUserRequestDto
        {
            Name = "Alice",
            Surname = "Williams",
            Email = "alice@example.com",
            Password = "SecurePass123!",
            Role = RoleEnum.Administrator,
        };

        // Act
        var user = dto.ToDomain();

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual("Alice", user.Name);
        Assert.AreEqual("Williams", user.Surname);
        Assert.AreEqual("alice@example.com", user.Email);
        Assert.AreEqual(1, user.Roles.Count);
        Assert.IsTrue(user.Roles.Contains(RoleEnum.Administrator));
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfUsersToListOfDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User("User1", "Last1", "user1@example.com", "Pass123!", RoleEnum.Administrator),
            new User("User2", "Last2", "user2@example.com", "Pass123!", RoleEnum.Operator),
        };

        // Act
        var dtos = users.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("User1", dtos[0].Name);
        Assert.AreEqual("User2", dtos[1].Name);
    }

    [TestMethod]
    public void ToListDto_Collection_ShouldMapListOfUsersToListOfListDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User("User1", "Last1", "user1@example.com", "Pass123!", RoleEnum.Administrator),
            new User("User2", "Last2", "user2@example.com", "Pass123!", RoleEnum.Operator),
            new User("User3", "Last3", "user3@example.com", "Pass123!", RoleEnum.Administrator),
        };

        // Act
        var dtos = users.ToListDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(3, dtos.Count);
        Assert.AreEqual("User1", dtos[0].Name);
        Assert.AreEqual("User2", dtos[1].Name);
        Assert.AreEqual("User3", dtos[2].Name);
    }

    [TestMethod]
    public void ToDomain_ShouldCreateVisitor_WhenRoleIsVisitorAndBirthDateProvided()
    {
        // Arrange
        var birthDate = new DateTime(1990, 5, 15);
        var dto = new CreateUserRequestDto
        {
            Name = "Maria",
            Surname = "Garcia",
            Email = "maria@example.com",
            Password = "SecurePass123!",
            Role = RoleEnum.Visitor,
            BirthDate = birthDate,
        };

        // Act
        var user = dto.ToDomain();

        // Assert
        Assert.IsNotNull(user);
        Assert.IsInstanceOfType(user, typeof(Visitor));
        var visitor = (Visitor)user;
        Assert.AreEqual("Maria", visitor.Name);
        Assert.AreEqual("Garcia", visitor.Surname);
        Assert.AreEqual("maria@example.com", visitor.Email);
        Assert.AreEqual(birthDate, visitor.BirthDate);
        Assert.AreEqual(MembershipLevel.Standard, visitor.MembershipLevel);
        Assert.AreNotEqual(Guid.Empty, visitor.NfcId);
        Assert.IsTrue(visitor.Roles.Contains(RoleEnum.Visitor));
    }

    [TestMethod]
    public void ToDomain_ShouldThrowException_WhenRoleIsVisitorAndBirthDateIsNull()
    {
        // Arrange
        var dto = new CreateUserRequestDto
        {
            Name = "Pedro",
            Surname = "Martinez",
            Email = "pedro@example.com",
            Password = "SecurePass123!",
            Role = RoleEnum.Visitor,
            BirthDate = null,
        };

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentException>(() => dto.ToDomain());
        Assert.IsTrue(exception.Message.Contains("BirthDate es requerido cuando el rol es Visitor"));
    }
}
