// <copyright file="VisitorsControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Visitors;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;
using VirtualPark.DTOs.Scoring.Responses;
using VirtualPark.DTOs.Visitors.Requests;
using VirtualPark.Infrastructure.Security;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class VisitorsControllerTests
{
    private Mock<IVisitorRepository> mockVisitorRepository = null!;
    private Mock<ITicketRepository> mockTicketRepository = null!;
    private Mock<ISpecialEventRepository> mockSpecialEventRepository = null!;
    private Mock<IPasswordHasher> mockPasswordHasher = null!;
    private VisitorService visitorService = null!;
    private IScoreHistoryService scoreHistoryService = null!;
    private VisitorsController controller = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockVisitorRepository = new Mock<IVisitorRepository>(MockBehavior.Strict);
        this.mockTicketRepository = new Mock<ITicketRepository>(MockBehavior.Strict);
        this.mockSpecialEventRepository = new Mock<ISpecialEventRepository>(MockBehavior.Strict);
        this.mockPasswordHasher = new Mock<IPasswordHasher>(MockBehavior.Strict);

        this.visitorService = new VisitorService(
            this.mockVisitorRepository.Object,
            this.mockPasswordHasher.Object);

        this.controller = new VisitorsController(this.visitorService, this.scoreHistoryService!);
    }

    [TestMethod]
    public void UpdateProfile_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var request = new UpdateVisitorProfileRequestDto
        {
            Name = "Juan",
            Surname = "Pérez",
            Email = "juan.perez@example.com",
            Password = "NewPassword123!",
        };

        var existingVisitor = new Visitor("OldName", "OldSurname", "old@example.com", "OldHashedPassword",
            DateTime.Now.AddYears(-30), MembershipLevel.Standard, Guid.NewGuid());

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockPasswordHasher
            .Setup(h => h.Hash(request.Password))
            .Returns("HashedPassword123");

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.Is<Visitor>(v =>
                v.Name == request.Name &&
                v.Surname == request.Surname &&
                v.Email == request.Email &&
                v.Password == "HashedPassword123")))
            .Returns(true)
            .Verifiable();

        // Act
        var result = this.controller.UpdateProfile(visitorId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);

        this.mockVisitorRepository.Verify();
        this.mockPasswordHasher.Verify();
    }

    [TestMethod]
    public void UpdateProfile_WithValidDataWithoutPassword_ReturnsNoContent()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var request = new UpdateVisitorProfileRequestDto
        {
            Name = "Juan",
            Surname = "Pérez",
            Email = "juan.perez@example.com",
            Password = null,
        };

        var existingVisitor = new Visitor("OldName", "OldSurname", "old@example.com", "ExistingHashedPassword",
            DateTime.Now.AddYears(-30), MembershipLevel.Standard, Guid.NewGuid());

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.Is<Visitor>(v =>
                v.Name == request.Name &&
                v.Surname == request.Surname &&
                v.Email == request.Email &&
                v.Password == "ExistingHashedPassword")))
            .Returns(true)
            .Verifiable();

        // Act
        var result = this.controller.UpdateProfile(visitorId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);

        this.mockVisitorRepository.Verify();
    }

    [TestMethod]
    public void UpdateProfile_WhenVisitorNotFound_ReturnsNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var request = new UpdateVisitorProfileRequestDto
        {
            Name = "Juan",
            Surname = "Pérez",
            Email = "juan.perez@example.com",
            Password = "NewPassword123!",
        };

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns((Visitor?)null);

        // Act
        var result = this.controller.UpdateProfile(visitorId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var responseValue = notFoundResult.Value;
        Assert.IsNotNull(responseValue);

        var messageProperty = responseValue.GetType().GetProperty("message");
        Assert.IsNotNull(messageProperty);
        var message = messageProperty.GetValue(responseValue) as string;
        Assert.IsTrue(message?.Contains("Visitor not found") == true);

        this.mockVisitorRepository.Verify();
    }

    [TestMethod]
    public void UpdateProfile_WithLongPassword_ReturnsNoContent()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var longPassword = new string('a', 200);
        var request = new UpdateVisitorProfileRequestDto
        {
            Name = "Juan",
            Surname = "Pérez",
            Email = "juan.perez@example.com",
            Password = longPassword,
        };

        var existingVisitor = new Visitor("OldName", "OldSurname", "old@example.com", "OldHashedPassword",
            DateTime.Now.AddYears(-30), MembershipLevel.Standard, Guid.NewGuid());

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockPasswordHasher
            .Setup(h => h.Hash(longPassword))
            .Returns("HashedLongPassword");

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.Is<Visitor>(v =>
                v.Name == request.Name &&
                v.Surname == request.Surname &&
                v.Email == request.Email &&
                v.Password == "HashedLongPassword")))
            .Returns(true)
            .Verifiable();

        // Act
        var result = this.controller.UpdateProfile(visitorId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<NoContentResult>(result);

        this.mockVisitorRepository.Verify();
        this.mockPasswordHasher.Verify();
    }

    [TestMethod]
    public void GetScoreHistory_WithValidVisitorId_ReturnsOk()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var scoreHistory = new List<ScoreHistoryEntryDto>
        {
            new ScoreHistoryEntryDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Description = "Atracción",
                PointsAwarded = 100,
                StrategyUsed = "ScoreByAttractionType",
                Origin = "Atracción",
            },
        };

        var mockScoreHistoryService = new Mock<IScoreHistoryService>();
        mockScoreHistoryService
            .Setup(s => s.GetVisitorHistory(visitorId))
            .Returns(scoreHistory);

        var controller = new VisitorsController(null!, mockScoreHistoryService.Object);

        // Act
        var result = controller.GetScoreHistory(visitorId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(scoreHistory, okResult.Value);

        mockScoreHistoryService.Verify(s => s.GetVisitorHistory(visitorId), Times.Once);
    }

    [TestMethod]
    public void GetScoreHistory_WithInvalidVisitorId_ReturnsNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var errorMessage = "Visitor not found";

        var mockScoreHistoryService = new Mock<IScoreHistoryService>();
        mockScoreHistoryService
            .Setup(s => s.GetVisitorHistory(visitorId))
            .Throws(new KeyNotFoundException(errorMessage));

        var controller = new VisitorsController(null!, mockScoreHistoryService.Object);

        // Act
        var result = controller.GetScoreHistory(visitorId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var responseValue = notFoundResult.Value;
        Assert.IsNotNull(responseValue);

        var messageProperty = responseValue.GetType().GetProperty("message");
        Assert.IsNotNull(messageProperty);
        var message = messageProperty.GetValue(responseValue) as string;
        Assert.AreEqual(errorMessage, message);

        mockScoreHistoryService.Verify(s => s.GetVisitorHistory(visitorId), Times.Once);
    }
}
