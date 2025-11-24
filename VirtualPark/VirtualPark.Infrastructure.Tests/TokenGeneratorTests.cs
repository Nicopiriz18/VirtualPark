// <copyright file="TokenGeneratorTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Security;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class TokenGeneratorTests
{
    private Mock<ISessionRepository> sessionRepositoryMock = null!;
    private TokenGenerator tokenGenerator = null!;

    [TestInitialize]
    public void Setup()
    {
        this.sessionRepositoryMock = new Mock<ISessionRepository>();
        this.tokenGenerator = new TokenGenerator(this.sessionRepositoryMock.Object);
    }

    [TestMethod]
    public void GenerateToken_ShouldReturnNonEmptyToken()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>())).Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsTrue(token.Length > 0);
    }

    [TestMethod]
    public void GenerateToken_ShouldReturnBase64FormattedToken()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>())).Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.IsNotNull(token);

        // Verify it's valid Base64 by trying to convert it back
        try
        {
            var bytes = Convert.FromBase64String(token);
            Assert.IsTrue(bytes.Length > 0);
        }
        catch (FormatException)
        {
            Assert.Fail("Token is not valid Base64 format");
        }
    }

    [TestMethod]
    public void GenerateToken_ShouldCreateSessionInRepository()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        Session? capturedSession = null;
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => capturedSession = s)
            .Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        this.sessionRepositoryMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Once);
        Assert.IsNotNull(capturedSession);
    }

    [TestMethod]
    public void GenerateToken_ShouldCreateSessionWithCorrectUserId()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        Session? capturedSession = null;
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => capturedSession = s)
            .Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.IsNotNull(capturedSession);
        Assert.AreEqual(user.Id, capturedSession.UserId);
    }

    [TestMethod]
    public void GenerateToken_ShouldCreateSessionWithMatchingToken()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        Session? capturedSession = null;
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => capturedSession = s)
            .Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.IsNotNull(capturedSession);
        Assert.AreEqual(token, capturedSession.Token);
    }

    [TestMethod]
    public void GenerateToken_ShouldGenerateUniqueTokens()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>())).Returns((Session s) => s);

        // Act
        var token1 = this.tokenGenerator.GenerateToken(user);
        var token2 = this.tokenGenerator.GenerateToken(user);
        var token3 = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.AreNotEqual(token1, token2);
        Assert.AreNotEqual(token1, token3);
        Assert.AreNotEqual(token2, token3);
    }

    [TestMethod]
    public void GenerateToken_ShouldCreateSessionWithValidId()
    {
        // Arrange
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        Session? capturedSession = null;
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
            .Callback<Session>(s => capturedSession = s)
            .Returns((Session s) => s);

        // Act
        this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.IsNotNull(capturedSession);
        Assert.AreNotEqual(Guid.Empty, capturedSession.Id);
    }

    [TestMethod]
    public void GenerateToken_TokenShouldBe44CharactersLong()
    {
        // Arrange - 32 bytes in Base64 = 44 characters
        var user = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);
        this.sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>())).Returns((Session s) => s);

        // Act
        var token = this.tokenGenerator.GenerateToken(user);

        // Assert
        Assert.AreEqual(44, token.Length);
    }
}
