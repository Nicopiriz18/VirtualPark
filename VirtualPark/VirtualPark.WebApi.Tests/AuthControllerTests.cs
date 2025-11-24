// <copyright file="AuthControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Session;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Auth.Requests;
using VirtualPark.DTOs.Auth.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class AuthControllerTests
{
    [TestMethod]
    public void Register_ValidUser_ReturnsCreated()
    {
        var request = new RegisterRequestDto
        {
            Name = "Ana",
            Surname = "Gomez",
            Email = "ana@test.com",
            Password = "password123",
            Birthday = new DateTime(1990, 1, 1),
        };

        var expectedId = Guid.NewGuid();
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.Register("Ana", "Gomez", "ana@test.com", "password123", It.IsAny<DateTime>()))
            .Returns((expectedId, "ana@test.com", "Ana", "Gomez"));

        var sessionServiceMock = new Mock<ISessionService>();
        var controller = new AuthController(mockService.Object, sessionServiceMock.Object);

        var result = controller.Register(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var response = result.Value as RegisterResponseDto;
        Assert.IsNotNull(response);
        Assert.AreEqual(expectedId, response.Id);
        Assert.AreEqual("ana@test.com", response.Email);
        Assert.AreEqual("Ana Gomez", response.FullName);
        Assert.AreEqual($"/api/v1/users/{expectedId}", result.Location);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsOkWithAuthResult()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.Login("test@example.com", "password123"))
            .Returns("fake-jwt-token");

        var sessionServiceMock = new Mock<ISessionService>();
        var visitor = new Visitor(
            "John",
            "Doe",
            "test@example.com",
            "hashed-password",
            new DateTime(1990, 1, 1),
            MembershipLevel.Standard,
            Guid.NewGuid());
        sessionServiceMock.Setup(s => s.GetByToken("fake-jwt-token")).Returns(visitor);

        var controller = new AuthController(mockService.Object, sessionServiceMock.Object);
        var loginRequest = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "password123",
        };

        // Act
        var result = controller.Login(loginRequest) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var authResult = result.Value as AuthResultDto;
        Assert.IsNotNull(authResult);
        Assert.AreEqual("fake-jwt-token", authResult.Token);
        CollectionAssert.Contains(authResult.Roles, RoleEnum.Visitor);
        Assert.AreEqual(visitor.Id, authResult.UserId);
        Assert.AreEqual(visitor.Email, authResult.Email);

        mockService.Verify(s => s.Login("test@example.com", "password123"), Times.Once);
        sessionServiceMock.Verify(s => s.GetByToken("fake-jwt-token"), Times.Once);
    }

    [TestMethod]
    public void Login_SessionReturnsNull_ReturnsOkWithEmptyRolesAndEmptyUser()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.Login("test@example.com", "password123"))
            .Returns("fake-jwt-token");

        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.GetByToken("fake-jwt-token")).Returns((User?)null);

        var controller = new AuthController(mockService.Object, sessionServiceMock.Object);
        var loginRequest = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "password123",
        };

        // Act
        var result = controller.Login(loginRequest) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var authResult = result.Value as AuthResultDto;
        Assert.IsNotNull(authResult);
        Assert.AreEqual("fake-jwt-token", authResult.Token);
        Assert.IsNotNull(authResult.Roles);
        Assert.AreEqual(0, authResult.Roles.Count);
        Assert.AreEqual(Guid.Empty, authResult.UserId);
        Assert.IsNull(authResult.Email);
    }

    [TestMethod]
    public void Login_UserWithMultipleRoles_ReturnsAllRoles()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.Login("multi@example.com", "pwd")).Returns("token-2");

        var sessionServiceMock = new Mock<ISessionService>();
        var user = new User("Name", "Surname", "multi@example.com", "pw", RoleEnum.Visitor);
        user.AssignRole(RoleEnum.Administrator);
        sessionServiceMock.Setup(s => s.GetByToken("token-2")).Returns(user);

        var controller = new AuthController(mockService.Object, sessionServiceMock.Object);
        var loginRequest = new LoginRequestDto
        {
            Email = "multi@example.com",
            Password = "pwd",
        };

        // Act
        var result = controller.Login(loginRequest) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var authResult = result.Value as AuthResultDto;
        Assert.IsNotNull(authResult);
        Assert.AreEqual("token-2", authResult.Token);
        CollectionAssert.Contains(authResult.Roles, RoleEnum.Visitor);
        CollectionAssert.Contains(authResult.Roles, RoleEnum.Administrator);
        Assert.AreEqual(user.Id, authResult.UserId);
        Assert.AreEqual(user.Email, authResult.Email);
    }
}
