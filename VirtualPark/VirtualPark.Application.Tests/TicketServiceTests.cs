// <copyright file="TicketServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using Moq;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Tests;

[TestClass]
public class TicketServiceTests
{
    private Mock<IVisitorRepository> mockVisitorRepository = null!;
    private Mock<ITicketRepository> mockTicketRepository = null!;
    private Mock<IPasswordHasher> mockPasswordHasher = null!;
    private Mock<ISpecialEventRepository> mockSpecialEventRepository = null!;
    private Mock<IMaintenanceRepository> mockMaintenanceRepository = null!;
    private TicketService ticketService = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockVisitorRepository = new Mock<IVisitorRepository>();
        this.mockTicketRepository = new Mock<ITicketRepository>();
        this.mockPasswordHasher = new Mock<IPasswordHasher>();
        this.mockSpecialEventRepository = new Mock<ISpecialEventRepository>();
        this.mockMaintenanceRepository = new Mock<IMaintenanceRepository>();
        this.mockMaintenanceRepository
            .Setup(m => m.GetByAttractionId(It.IsAny<Guid>()))
            .Returns(Enumerable.Empty<PreventiveMaintenance>());
        this.ticketService = new TicketService(
            this.mockTicketRepository.Object,
            this.mockVisitorRepository.Object,
            this.mockSpecialEventRepository.Object,
            this.mockMaintenanceRepository.Object);
    }

    [TestMethod]
    public void PurchaseTicket_WithValidData_ShouldCreateGeneralTicket()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("John", "Doe", "john@example.com", "pass123", new DateTime(1990, 1, 1),
            MembershipLevel.Standard, Guid.NewGuid());
        var visitDate = DateTime.Today.AddDays(5);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockTicketRepository
            .Setup(r => r.Add(It.IsAny<Ticket>()))
            .Returns((Ticket t) => t);

        // Act
        var ticket = this.ticketService.PurchaseTicket(visitorId, visitDate, TicketType.General);

        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(visitDate, ticket.VisitDate);
        Assert.AreEqual(TicketType.General, ticket.Type);
        Assert.AreNotEqual(Guid.Empty, ticket.QrCode);
        Assert.IsNull(ticket.SpecialEventId);
        Assert.AreEqual(visitorId, ticket.VisitorId);
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Once);
    }

    [TestMethod]
    public void PurchaseSpecialEventTicket_WithValidData_ShouldCreateSpecialEventTicket()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15),
            MembershipLevel.Standard, Guid.NewGuid());
        var visitDate = DateTime.Today.AddDays(10);
        var specialEventId = Guid.NewGuid();
        var specialEvent = new Domain.SpecialEvent("Concert", DateTime.Today.AddDays(10), 100, 50.0m);
        var attraction = new Attraction("Main Stage", "Stage", "Stage", 0, 100);
        specialEvent.AddAttraction(attraction);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns(specialEvent);

        this.mockTicketRepository
            .Setup(r => r.CountTicketsByEventId(specialEventId))
            .Returns(50); // 50 tickets already sold

        this.mockTicketRepository
            .Setup(r => r.Add(It.IsAny<Ticket>()))
            .Returns((Ticket t) => t);

        // Act
        var ticket = this.ticketService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId);

        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(visitDate, ticket.VisitDate);
        Assert.AreEqual(TicketType.SpecialEvent, ticket.Type);
        Assert.AreNotEqual(Guid.Empty, ticket.QrCode);
        Assert.AreEqual(specialEventId, ticket.SpecialEventId);
        Assert.AreEqual(visitorId, ticket.VisitorId);
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Once);
        this.mockTicketRepository.Verify(r => r.CountTicketsByEventId(specialEventId), Times.Once);
        this.mockMaintenanceRepository.Verify(m => m.GetByAttractionId(attraction.Id), Times.Once);
    }

    [TestMethod]
    public void PurchaseSpecialEventTicket_WhenAtCapacity_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15),
            MembershipLevel.Standard, Guid.NewGuid());
        var visitDate = DateTime.Today.AddDays(10);
        var specialEventId = Guid.NewGuid();
        var specialEvent = new Domain.SpecialEvent("Concert", DateTime.Today.AddDays(10), 100, 50.0m);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns(specialEvent);

        this.mockTicketRepository
            .Setup(r => r.CountTicketsByEventId(specialEventId))
            .Returns(100); // Event is at full capacity

        // Act & Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            this.ticketService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));

        Assert.IsTrue(exception.Message.Contains("maximum capacity"));
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    }

    [TestMethod]
    public void PurchaseSpecialEventTicket_WhenEventNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15),
            MembershipLevel.Standard, Guid.NewGuid());
        var visitDate = DateTime.Today.AddDays(10);
        var specialEventId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns((Domain.SpecialEvent?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            this.ticketService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));

        Assert.IsTrue(exception.Message.Contains("Special event not found"));
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    }

    [TestMethod]
    public void PurchaseSpecialEventTicket_WhenAttractionUnderMaintenance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15),
            MembershipLevel.Standard, Guid.NewGuid());
        var visitDate = DateTime.Today.AddDays(10);
        var specialEventId = Guid.NewGuid();
        var specialEvent = new Domain.SpecialEvent("Concert", visitDate, 100, 50.0m);
        var attraction = new Attraction("Main Stage", "Stage", "Stage", 0, 100);
        specialEvent.AddAttraction(attraction);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(visitor);

        this.mockSpecialEventRepository
            .Setup(r => r.GetById(specialEventId))
            .Returns(specialEvent);

        this.mockTicketRepository
            .Setup(r => r.CountTicketsByEventId(specialEventId))
            .Returns(10);

        var maintenance = new PreventiveMaintenance(attraction.Id, visitDate.Date, visitDate.TimeOfDay, TimeSpan.FromHours(2), "Maintenance");
        this.mockMaintenanceRepository
            .Setup(m => m.GetByAttractionId(attraction.Id))
            .Returns(new[] { maintenance });

        // Act & Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            this.ticketService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));

        Assert.IsTrue(exception.Message.Contains("mantenimiento"));
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
        this.mockMaintenanceRepository.Verify(m => m.GetByAttractionId(attraction.Id), Times.Once);
    }

    [TestMethod]
    public void PurchaseTicket_WhenVisitorNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = DateTime.Today.AddDays(5);

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns((Visitor?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            this.ticketService.PurchaseTicket(visitorId, visitDate, TicketType.General));

        Assert.IsTrue(exception.Message.Contains("Visitor not found"));
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    }

    [TestMethod]
    public void PurchaseSpecialEventTicket_WhenVisitorNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = DateTime.Today.AddDays(10);
        var specialEventId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns((Visitor?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            this.ticketService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));

        Assert.IsTrue(exception.Message.Contains("Visitor not found"));
        this.mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    }
}
