// <copyright file="TicketsControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;
using VirtualPark.DTOs.Tickets.Requests;
using VirtualPark.DTOs.Tickets.Responses;
using VirtualPark.Infrastructure.Security;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class TicketsControllerTests
{
    private Mock<IVisitorRepository> mockVisitorRepository = null!;
    private Mock<ITicketRepository> mockTicketRepository = null!;
    private Mock<ISpecialEventRepository> mockSpecialEventRepository = null!;
    private Mock<IMaintenanceRepository> mockMaintenanceRepository = null!;
    private Mock<IPasswordHasher> mockPasswordHasher = null!;
    private TicketService ticketService = null!;
    private TicketsController controller = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockVisitorRepository = new Mock<IVisitorRepository>(MockBehavior.Strict);
        this.mockTicketRepository = new Mock<ITicketRepository>(MockBehavior.Strict);
        this.mockSpecialEventRepository = new Mock<ISpecialEventRepository>(MockBehavior.Strict);
        this.mockMaintenanceRepository = new Mock<IMaintenanceRepository>(MockBehavior.Strict);
        this.mockPasswordHasher = new Mock<IPasswordHasher>(MockBehavior.Strict);

        this.ticketService = new TicketService(
            this.mockTicketRepository.Object,
            this.mockVisitorRepository.Object,
            this.mockSpecialEventRepository.Object,
            this.mockMaintenanceRepository.Object);

        this.controller = new TicketsController(this.ticketService);
    }

    [TestMethod]
    public void PurchaseTicket_GeneralTicket_ReturnsCreatedWithTicketResponse()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = DateTime.Now.AddDays(7);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = visitDate,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());
        var ticketId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockTicketRepository
            .Setup(r => r.Add(It.Is<Ticket>(t =>
                t.VisitorId == visitorId &&
                t.VisitDate == visitDate &&
                t.Type == TicketType.General &&
                t.SpecialEventId == null)))
            .Returns((Ticket t) =>
            {
                var idProperty = typeof(Ticket).GetProperty("Id");
                idProperty?.SetValue(t, ticketId);
                return t;
            });

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);

        var response = createdResult.Value as TicketDto;
        Assert.IsNotNull(response);
        Assert.AreEqual(ticketId, response.Id);
        Assert.AreEqual(visitDate, response.VisitDate);
        Assert.AreEqual(TicketType.General, response.Type);

        this.mockVisitorRepository.Verify();
        this.mockTicketRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_SpecialEventTicket_ReturnsCreatedWithTicketResponse()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();
        var visitDate = DateTime.Now.AddDays(7);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = visitDate,
            Type = TicketType.SpecialEvent,
            SpecialEventId = specialEventId,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());
        var specialEvent = new SpecialEvent("Concierto de Rock", DateTime.Now.AddDays(7), 100, 50.0m);
        var ticketId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns(specialEvent);

        this.mockTicketRepository
            .Setup(r => r.CountTicketsByEventId(specialEventId))
            .Returns(50);

        this.mockTicketRepository
            .Setup(r => r.Add(It.Is<Ticket>(t =>
                t.VisitorId == visitorId &&
                t.VisitDate == visitDate &&
                t.Type == TicketType.SpecialEvent &&
                t.SpecialEventId == specialEventId)))
            .Returns((Ticket t) =>
            {
                var idProperty = typeof(Ticket).GetProperty("Id");
                idProperty?.SetValue(t, ticketId);
                return t;
            });

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);

        var response = createdResult.Value as TicketDto;
        Assert.IsNotNull(response);
        Assert.AreEqual(ticketId, response.Id);
        Assert.AreEqual(visitDate, response.VisitDate);
        Assert.AreEqual(TicketType.SpecialEvent, response.Type);
        Assert.AreEqual(specialEventId, response.SpecialEventId);

        this.mockVisitorRepository.Verify();
        this.mockSpecialEventRepository.Verify();
        this.mockTicketRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_WithoutSpecialEventId_ReturnsBadRequest()
    {
        // Arrange
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = Guid.NewGuid(),
            VisitDate = DateTime.Now.AddDays(7),
            Type = TicketType.SpecialEvent,
            SpecialEventId = null,
        };

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public void PurchaseTicket_WhenVisitorNotFound_ReturnsNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = DateTime.Now.AddDays(7),
            Type = TicketType.General,
            SpecialEventId = null,
        };

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns((Visitor?)null);

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        this.mockVisitorRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_WhenEventNotFound_ReturnsNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();
        var visitDate = DateTime.Now.AddDays(7);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = visitDate,
            Type = TicketType.SpecialEvent,
            SpecialEventId = specialEventId,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns((SpecialEvent?)null);

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        this.mockVisitorRepository.Verify();
        this.mockSpecialEventRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_WhenEventAtCapacity_ReturnsBadRequest()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();
        var visitDate = DateTime.Now.AddDays(7);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = visitDate,
            Type = TicketType.SpecialEvent,
            SpecialEventId = specialEventId,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());
        var specialEvent = new SpecialEvent("Concierto de Rock", DateTime.Now.AddDays(7), 100, 50.0m);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns(specialEvent);

        this.mockTicketRepository
            .Setup(r => r.CountTicketsByEventId(specialEventId))
            .Returns(100);

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        this.mockVisitorRepository.Verify();
        this.mockSpecialEventRepository.Verify();
        this.mockTicketRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_WithFutureDate_ReturnsCreated()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var futureDate = DateTime.Now.AddMonths(3);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = futureDate,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());
        var ticketId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockTicketRepository
            .Setup(r => r.Add(It.IsAny<Ticket>()))
            .Returns((Ticket t) =>
            {
                var idProperty = typeof(Ticket).GetProperty("Id");
                idProperty?.SetValue(t, ticketId);
                return t;
            });

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);

        this.mockVisitorRepository.Verify();
        this.mockTicketRepository.Verify();
    }

    [TestMethod]
    public void PurchaseTicket_WithPastDate_ShouldStillCallService()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var pastDate = DateTime.Now.AddDays(-10);
        var request = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = pastDate,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        var visitor = new Visitor("Juan", "Pérez", "juan@example.com", "hashedpassword", DateTime.Now.AddYears(-25), MembershipLevel.Standard, Guid.NewGuid());
        var ticketId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockTicketRepository
            .Setup(r => r.Add(It.Is<Ticket>(t => t.VisitDate == pastDate)))
            .Returns((Ticket t) =>
            {
                var idProperty = typeof(Ticket).GetProperty("Id");
                idProperty?.SetValue(t, ticketId);
                return t;
            });

        // Act
        var result = this.controller.PurchaseTicket(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

        this.mockVisitorRepository.Verify();
        this.mockTicketRepository.Verify();
    }
}
