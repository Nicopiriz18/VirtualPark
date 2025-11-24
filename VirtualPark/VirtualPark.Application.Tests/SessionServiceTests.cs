// <copyright file="SessionServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Session;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class SessionServiceTests
{
    private Mock<IUserService> mockUserService = null!;
    private Mock<ISessionRepository> mockSessionRepository = null!;
    private SessionService sessionService = null!;
    private User testUser = null!;
    private User adminUser = null!;
    private User operatorUser = null!;
    private Dictionary<string, Domain.Session> sessions = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockUserService = new Mock<IUserService>();
        this.mockSessionRepository = new Mock<ISessionRepository>();
        this.sessions = [];

        // Setup repository to store and retrieve sessions
        this.mockSessionRepository.Setup(x => x.Add(It.IsAny<Domain.Session>()))
            .Returns((Domain.Session s) =>
            {
                this.sessions[s.Token] = s;
                return s;
            });

        this.mockSessionRepository.Setup(x => x.GetByToken(It.IsAny<string>()))
            .Returns((string token) => this.sessions.ContainsKey(token) ? this.sessions[token] : null);

        this.sessionService = new SessionService(this.mockUserService.Object, this.mockSessionRepository.Object);

        // Create test users
        this.testUser = new User("John", "Doe", "john.doe@test.com", "password123", RoleEnum.Administrator);

        this.adminUser = new User("Admin", "User", "admin@test.com", "admin123", RoleEnum.Administrator);
        this.adminUser.AssignRole(RoleEnum.Administrator);

        this.operatorUser = new User("Operator", "User", "operator@test.com", "operator123", RoleEnum.Operator);
        this.operatorUser.AssignRole(RoleEnum.Operator);
    }

    [TestMethod]
    public void CreateSession_ValidUserIdAndToken_ReturnsSession()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "test-token-123";

        // Act
        var session = this.sessionService.CreateSession(userId, token);

        // Assert
        Assert.IsNotNull(session);
        Assert.AreEqual(userId, session.UserId);
        Assert.AreEqual(token, session.Token);
        Assert.AreNotEqual(Guid.Empty, session.Id);
    }

    [TestMethod]
    public void CreateSession_DifferentUsers_CreatesDifferentSessions()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var token1 = "token-1";
        var token2 = "token-2";

        // Act
        var session1 = this.sessionService.CreateSession(userId1, token1);
        var session2 = this.sessionService.CreateSession(userId2, token2);

        // Assert
        Assert.IsNotNull(session1);
        Assert.IsNotNull(session2);
        Assert.AreNotEqual(session1.Id, session2.Id);
        Assert.AreNotEqual(session1.Token, session2.Token);
        Assert.AreEqual(userId1, session1.UserId);
        Assert.AreEqual(userId2, session2.UserId);
    }

    [TestMethod]
    public void IsSessionValid_ValidTokenWithCorrectRole_ReturnsTrue()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.adminUser.Id)).Returns(this.adminUser);
        var session = this.sessionService.CreateSession(this.adminUser.Id, "admin-token");

        // Act
        var isValid = this.sessionService.IsSessionValid(session.Token, RoleEnum.Administrator);

        // Assert
        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void IsSessionValid_ValidTokenWithIncorrectRole_ReturnsFalse()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.operatorUser.Id)).Returns(this.operatorUser);
        var session = this.sessionService.CreateSession(this.operatorUser.Id, "operator-token");

        // Act
        var isValid = this.sessionService.IsSessionValid(session.Token, RoleEnum.Administrator);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void IsSessionValid_InvalidToken_ReturnsFalse()
    {
        // Act
        var isValid = this.sessionService.IsSessionValid("invalid-token", RoleEnum.Administrator);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void IsSessionValid_ValidTokenButUserNotFound_ReturnsFalse()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.testUser.Id)).Returns((User?)null);
        var session = this.sessionService.CreateSession(this.testUser.Id, "test-token");

        // Act
        var isValid = this.sessionService.IsSessionValid(session.Token, RoleEnum.Administrator);

        // Assert
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void GetByToken_ValidToken_ReturnsUser()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.testUser.Id)).Returns(this.testUser);
        var session = this.sessionService.CreateSession(this.testUser.Id, "test-token");

        // Act
        var user = this.sessionService.GetByToken(session.Token);

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual(this.testUser.Id, user.Id);
        Assert.AreEqual(this.testUser.Email, user.Email);
    }

    [TestMethod]
    public void GetByToken_InvalidToken_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(
            () => this.sessionService.GetByToken("invalid-token"));

        Assert.AreEqual("Session not found.", exception.Message);
    }

    [TestMethod]
    public void GetByToken_ValidTokenButUserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.testUser.Id)).Returns(default(User)!);
        var session = this.sessionService.CreateSession(this.testUser.Id, "test-token");

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(
            () => this.sessionService.GetByToken(session.Token));

        Assert.AreEqual("User not found.", exception.Message);
    }

    [TestMethod]
    public void IsSessionValid_OperatorWithOperatorRole_ReturnsTrue()
    {
        // Arrange
        this.mockUserService.Setup(x => x.GetById(this.operatorUser.Id)).Returns(this.operatorUser);
        var session = this.sessionService.CreateSession(this.operatorUser.Id, "operator-token");

        // Act
        var isValid = this.sessionService.IsSessionValid(session.Token, RoleEnum.Operator);

        // Assert
        Assert.IsTrue(isValid);
    }
}
