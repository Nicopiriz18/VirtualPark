// <copyright file="UserControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Users.Requests;
using VirtualPark.DTOs.Users.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class UserControllerTests
{
    [TestMethod]
    public void GetAll_Returns_Ok_With_Users()
    {
        var user = new User(
            name: "Test User",
            surname: "Test Surname",
            email: "a@!.com",
            password: "hashedpassword",
            role: RoleEnum.Operator);
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        mockService.Setup(service => service.GetAll()).Returns([user]);
        var controller = new UsersController(mockService.Object);
        var result = controller.Get() as OkObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var users = result.Value as IEnumerable<UserListDto>;
        Assert.IsNotNull(users);
        var userList = users.ToList();
        Assert.AreEqual(1, userList.Count);
        Assert.AreEqual("Test User", userList.First().Name);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetById_Returns_Ok_With_User()
    {
        var user = new User(
            name: "Test User",
            surname: "Test Surname",
            email: "a@!.com",
            password: "hashedpassword",
            role: RoleEnum.Operator);
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        var userId = Guid.NewGuid();
        mockService.Setup(service => service.GetById(userId)).Returns(user);
        var controller = new UsersController(mockService.Object);
        var result = controller.GetById(userId) as OkObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var returnedUserDto = result.Value as UserDetailDto;
        Assert.IsNotNull(returnedUserDto);
        Assert.AreEqual("Test User", returnedUserDto.Name);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void Create_ReturnsCreated_WhenValidUser()
    {
        var newUser = new User("Nuevo", "User", "nuevo@ort.edu", "hashedpass", RoleEnum.Administrator);
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.Create(It.IsAny<User>())).Returns(newUser);

        var controller = new UsersController(mockService.Object);

        var request = new CreateUserRequestDto
        {
            Name = "Nuevo",
            Surname = "User",
            Email = "nuevo@ort.edu",
            Password = "hashedpass",
            Role = RoleEnum.Administrator,
        };
        var result = controller.Post(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(nameof(controller.GetById), result.ActionName);
        Assert.IsNotNull(result.RouteValues);
        var userDto = result.Value as UserDto;
        Assert.IsNotNull(userDto);
        Assert.AreEqual("Nuevo", userDto.Name);
    }

    [TestMethod]
    public void Create_ReturnsCreated_WhenServiceReturnsNull()
    {
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.Create(It.IsAny<User>())).Returns((User?)null);

        var controller = new UsersController(mockService.Object);

        var request = new CreateUserRequestDto
        {
            Name = "Nuevo",
            Surname = "User",
            Email = "nuevo@ort.edu",
            Password = "hashedpass",
            Role = RoleEnum.Operator,
        };
        var result = controller.Post(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.IsNull(result.Value);
        Assert.AreEqual(nameof(controller.Get), result.ActionName);
        Assert.IsNull(result.RouteValues);
    }

    [TestMethod]
    public void Update_ReturnsNoContent_WhenSuccessful()
    {
        var mockService = new Mock<IUserService>();
        var userId = Guid.NewGuid();

        mockService.Setup(s => s.Update(userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var controller = new UsersController(mockService.Object);

        var request = new UpdateUserRequestDto
        {
            Name = "Test",
            Surname = "User",
            Email = "test@ort.edu",
            Password = null,
        };

        var result = controller.Put(userId, request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.Verify(s => s.Update(userId, "Test", "User", "test@ort.edu", null), Times.Once);
    }

    [TestMethod]
    public void AssignRole_ReturnsNoContent_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var role = RoleEnum.Administrator;
        var request = new AssignRoleRequestDto { Role = role };
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        mockService.Setup(s => s.AssignRole(userId, role, It.IsAny<DateTime?>(), It.IsAny<MembershipLevel?>()));

        var controller = new UsersController(mockService.Object);

        var result = controller.AssignRole(userId, request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = RoleEnum.Operator;
        var request = new AssignRoleRequestDto { Role = role };
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        mockService.Setup(s => s.AssignRole(userId, role, It.IsAny<DateTime?>(), It.IsAny<MembershipLevel?>()))
            .Throws(new KeyNotFoundException($"User with id {userId} was not found."));

        var controller = new UsersController(mockService.Object);

        // Act
        var result = controller.AssignRole(userId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = RoleEnum.Administrator;
        var request = new AssignRoleRequestDto { Role = role };
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        mockService.Setup(s => s.AssignRole(userId, role, It.IsAny<DateTime?>(), It.IsAny<MembershipLevel?>()));

        var controller = new UsersController(mockService.Object);

        // Act
        _ = controller.AssignRole(userId, request);

        // Assert
        mockService.Verify(s => s.AssignRole(userId, role, It.IsAny<DateTime?>(), It.IsAny<MembershipLevel?>()), Times.Once);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_WorksWithOperatorRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = RoleEnum.Operator;
        var request = new AssignRoleRequestDto { Role = role };
        var mockService = new Mock<IUserService>(MockBehavior.Strict);
        mockService.Setup(s => s.AssignRole(userId, role, It.IsAny<DateTime?>(), It.IsAny<MembershipLevel?>()));

        var controller = new UsersController(mockService.Object);

        // Act
        var result = controller.AssignRole(userId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.VerifyAll();
    }

    [TestMethod]
    public void Create_ReturnsCreated_WhenVisitorWithBirthDate()
    {
        // Arrange
        var birthDate = new DateTime(1995, 6, 10);
        var visitor = new Visitor("Laura", "Martinez", "laura@ort.edu", "hashedpass", birthDate, MembershipLevel.Standard, Guid.NewGuid());
        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.Create(It.IsAny<Visitor>())).Returns(visitor);

        var controller = new UsersController(mockService.Object);

        var request = new CreateUserRequestDto
        {
            Name = "Laura",
            Surname = "Martinez",
            Email = "laura@ort.edu",
            Password = "hashedpass",
            Role = RoleEnum.Visitor,
            BirthDate = birthDate,
        };

        // Act
        var result = controller.Post(request) as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(nameof(controller.GetById), result.ActionName);
        var userDto = result.Value as UserDto;
        Assert.IsNotNull(userDto);
        Assert.AreEqual("Laura", userDto.Name);
    }

    [TestMethod]
    public void Create_ReturnsBadRequest_WhenVisitorWithoutBirthDate()
    {
        // Arrange
        var mockService = new Mock<IUserService>();
        var controller = new UsersController(mockService.Object);

        var request = new CreateUserRequestDto
        {
            Name = "Pedro",
            Surname = "Gomez",
            Email = "pedro@ort.edu",
            Password = "hashedpass",
            Role = RoleEnum.Visitor,
            BirthDate = null,
        };

        // Act
        var result = controller.Post(request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.IsNotNull(result.Value);
    }

    [TestMethod]
    public void AssignRole_ReturnsNoContent_WhenAssigningVisitorRoleWithBirthDate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var birthDate = new DateTime(1992, 8, 25);
        var request = new AssignRoleRequestDto
        {
            Role = RoleEnum.Visitor,
            BirthDate = birthDate,
            MembershipLevel = MembershipLevel.Standard,
        };

        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.AssignRole(userId, RoleEnum.Visitor, birthDate, MembershipLevel.Standard));

        var controller = new UsersController(mockService.Object);

        // Act
        var result = controller.AssignRole(userId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.Verify(s => s.AssignRole(userId, RoleEnum.Visitor, birthDate, MembershipLevel.Standard), Times.Once);
    }

    [TestMethod]
    public void AssignRole_ReturnsBadRequest_WhenAssigningVisitorRoleWithoutBirthDateToNonVisitor()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonVisitorUser = new User("Maria", "Sanchez", "maria@example.com", "pass", RoleEnum.Operator);
        var request = new AssignRoleRequestDto
        {
            Role = RoleEnum.Visitor,
            BirthDate = null,
        };

        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.GetById(userId)).Returns(nonVisitorUser);

        var controller = new UsersController(mockService.Object);

        // Act
        var result = controller.AssignRole(userId, request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.IsNotNull(result.Value);
        mockService.Verify(s => s.GetById(userId), Times.Once);
    }

    [TestMethod]
    public void AssignRole_ReturnsNoContent_WhenAssigningVisitorRoleToExistingVisitor()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingVisitor = new Visitor("Carmen", "Torres", "carmen@example.com", "pass", new DateTime(1988, 12, 5), MembershipLevel.Premium, Guid.NewGuid());
        var request = new AssignRoleRequestDto
        {
            Role = RoleEnum.Visitor,
            BirthDate = null, // Not needed for existing visitor
        };

        var mockService = new Mock<IUserService>();
        mockService.Setup(s => s.GetById(userId)).Returns(existingVisitor);
        mockService.Setup(s => s.AssignRole(userId, RoleEnum.Visitor, null, null));

        var controller = new UsersController(mockService.Object);

        // Act
        var result = controller.AssignRole(userId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.Verify(s => s.GetById(userId), Times.Once);
        mockService.Verify(s => s.AssignRole(userId, RoleEnum.Visitor, null, null), Times.Once);
    }
}
