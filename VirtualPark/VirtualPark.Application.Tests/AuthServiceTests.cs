// <copyright file="AuthServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Session;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Exceptions;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Tests;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> userRepositoryMock = null!;
    private Mock<IPasswordHasher> passwordHasherMock = null!;
    private Mock<ITokenGenerator> tokenGeneratorMock = null!;
    private Mock<ISessionService> sessionServiceMock = null!;
    private AuthService authService = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userRepositoryMock = new Mock<IUserRepository>();
        this.passwordHasherMock = new Mock<IPasswordHasher>();
        this.tokenGeneratorMock = new Mock<ITokenGenerator>();
        this.sessionServiceMock = new Mock<ISessionService>();

        this.authService = new AuthService(
            this.userRepositoryMock.Object,
            this.passwordHasherMock.Object,
            this.tokenGeneratorMock.Object,
            this.sessionServiceMock.Object);
    }

    [TestMethod]
    public void Register_ShouldHashPassword_WhenPasswordIsNotNull()
    {
        // Arrange
        var name = "John";
        var surname = "Doe";
        var email = "john.doe@test.com";
        var password = "password123";
        var birthday = new DateTime(1990, 1, 1);

        var hashedPassword = "hashedPassword123";
        var createdUser = new Visitor(name, surname, email, hashedPassword, birthday, MembershipLevel.Standard, Guid.NewGuid());

        this.passwordHasherMock.Setup(p => p.Hash(password)).Returns(hashedPassword);
        this.userRepositoryMock.Setup(r => r.Add(It.IsAny<Visitor>())).Returns(createdUser);

        // Act
        this.authService.Register(name, surname, email, password, birthday);

        // Assert
        this.passwordHasherMock.Verify(p => p.Hash(password), Times.Once);
    }

    [TestMethod]
    public void Register_ShouldAddUserToRepository()
    {
        // Arrange
        var name = "John";
        var surname = "Doe";
        var email = "john.doe@test.com";
        var password = "password123";
        var birthday = new DateTime(1990, 1, 1);

        var hashedPassword = "hashedPassword123";
        var createdUser = new Visitor(name, surname, email, hashedPassword, birthday, MembershipLevel.Standard, Guid.NewGuid());

        this.passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns(hashedPassword);
        this.userRepositoryMock.Setup(r => r.Add(It.IsAny<Visitor>())).Returns(createdUser);

        // Act
        this.authService.Register(name, surname, email, password, birthday);

        // Assert
        this.userRepositoryMock.Verify(
            r => r.Add(It.Is<Visitor>(u =>
            u.Name == name &&
            u.Surname == surname &&
            u.Email == email &&
            u.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void Register_ShouldReturnUserWithoutPassword()
    {
        // Arrange
        var name = "John";
        var surname = "Doe";
        var email = "john.doe@test.com";
        var password = "password123";
        var birthday = new DateTime(1990, 1, 1);

        var createdUser = new Visitor(name, surname, email, "hashedPassword", birthday, MembershipLevel.Standard, Guid.NewGuid());

        this.passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashedPassword");
        this.userRepositoryMock.Setup(r => r.Add(It.IsAny<Visitor>())).Returns(createdUser);

        // Act
        var result = this.authService.Register(name, surname, email, password, birthday);

        // Assert - Verifica que la tupla retornada no contiene Password
        Assert.IsNotNull(result);
        Assert.AreEqual(createdUser.Id, result.Id);
        Assert.AreEqual(email, result.Email);
        Assert.AreEqual(name, result.Name);
        Assert.AreEqual(surname, result.Surname);

        // La tupla por diseño no puede contener Password
    }

    [TestMethod]
    public void Register_ShouldReturnCorrectUserData()
    {
        // Arrange
        var name = "John";
        var surname = "Doe";
        var email = "john.doe@test.com";
        var password = "password123";
        var birthday = new DateTime(1990, 1, 1);

        var createdUser = new Visitor(name, surname, email, "hashedPassword", birthday, MembershipLevel.Standard, Guid.NewGuid());

        this.passwordHasherMock.Setup(p => p.Hash(password)).Returns("hashedPassword");
        this.userRepositoryMock.Setup(r => r.Add(It.IsAny<Visitor>())).Returns(createdUser);

        // Act
        var result = this.authService.Register(name, surname, email, password, birthday);

        // Assert
        Assert.AreEqual(createdUser.Id, result.Id);
        Assert.AreEqual(email, result.Email);
        Assert.AreEqual(name, result.Name);
        Assert.AreEqual(surname, result.Surname);
    }

    [TestMethod]
    public void Login_ShouldCallPasswordVerify_WhenUserIsFound()
    {
        // Arrange
        var email = "john.doe@test.com";
        var password = "password123";
        var hashedPassword = "hashedPassword123";

        var user = new User("John", "Doe", email, hashedPassword, RoleEnum.Administrator);

        this.userRepositoryMock.Setup(r => r.GetByEmail(email)).Returns(user);
        this.passwordHasherMock.Setup(p => p.Verify(password, hashedPassword)).Returns(true);
        this.tokenGeneratorMock.Setup(t => t.GenerateToken(user)).Returns("token");

        // Act
        this.authService.Login(email, password);

        // Assert
        this.passwordHasherMock.Verify(p => p.Verify(password, hashedPassword), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCredentialsException))]
    public void Login_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var email = "nonexistent@test.com";
        var password = "password123";

        this.userRepositoryMock.Setup(r => r.GetByEmail(email)).Returns((User?)null);

        // Act
        this.authService.Login(email, password);

        // Assert - ExpectedException
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCredentialsException))]
    public void Login_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var email = "john.doe@test.com";
        var password = "wrongPassword";
        var hashedPassword = "hashedPassword123";

        var user = new User("John", "Doe", email, hashedPassword, RoleEnum.Administrator);

        this.userRepositoryMock.Setup(r => r.GetByEmail(email)).Returns(user);
        this.passwordHasherMock.Setup(p => p.Verify(password, hashedPassword)).Returns(false);

        // Act
        this.authService.Login(email, password);

        // Assert - ExpectedException
    }

    [TestMethod]
    public void Login_ShouldCreateSession_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "john.doe@test.com";
        var password = "password123";
        var hashedPassword = "hashedPassword123";
        var expectedToken = "generatedToken123";

        var user = new User("John", "Doe", email, hashedPassword, RoleEnum.Administrator);

        this.userRepositoryMock.Setup(r => r.GetByEmail(email)).Returns(user);
        this.passwordHasherMock.Setup(p => p.Verify(password, hashedPassword)).Returns(true);
        this.tokenGeneratorMock.Setup(t => t.GenerateToken(user)).Returns(expectedToken);
        this.sessionServiceMock.Setup(s => s.CreateSession(user.Id, expectedToken))
            .Returns(new Domain.Session { Id = Guid.NewGuid(), UserId = user.Id, Token = expectedToken });

        // Act
        var result = this.authService.Login(email, password);

        // Assert
        this.sessionServiceMock.Verify(s => s.CreateSession(user.Id, expectedToken), Times.Once);
        Assert.AreEqual(expectedToken, result);
    }

    [TestMethod]
    public void Login_ShouldReturnCorrectToken()
    {
        // Arrange
        var email = "john.doe@test.com";
        var password = "password123";
        var hashedPassword = "hashedPassword123";
        var expectedToken = "generatedToken123";

        var user = new User("John", "Doe", email, hashedPassword, RoleEnum.Administrator);

        this.userRepositoryMock.Setup(r => r.GetByEmail(email)).Returns(user);
        this.passwordHasherMock.Setup(p => p.Verify(password, hashedPassword)).Returns(true);
        this.tokenGeneratorMock.Setup(t => t.GenerateToken(user)).Returns(expectedToken);

        // Act
        var token = this.authService.Login(email, password);

        // Assert
        Assert.AreEqual(expectedToken, token);
    }
}
