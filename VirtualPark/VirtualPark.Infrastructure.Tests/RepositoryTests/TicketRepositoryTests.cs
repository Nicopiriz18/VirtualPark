// <copyright file="TicketRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class TicketRepositoryTests
{
    private TicketRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        this.context = new ParkDbContext(options);
        this.repository = new TicketRepository(this.context);
    }

    [TestMethod]
    public void Add_ShouldAddNewTicket()
    {
        // Arrange
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);

        // Act
        var createdTicket = this.repository.Add(ticket);

        // Assert
        Assert.IsNotNull(createdTicket);
        Assert.AreEqual(ticket.VisitDate, createdTicket.VisitDate);
        Assert.AreEqual(ticket.Type, createdTicket.Type);
        Assert.AreEqual(ticket.QrCode, createdTicket.QrCode);
        Assert.IsNull(createdTicket.SpecialEventId);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllTickets()
    {
        // Arrange
        var ticket1 = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);
        var ticket2 = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(2),
            type: TicketType.SpecialEvent,
            qrCode: Guid.NewGuid(),
            specialEventId: Guid.NewGuid());

        this.repository.Add(ticket1);
        this.repository.Add(ticket2);

        // Act
        var allTickets = this.repository.GetAll().ToList();

        // Assert
        Assert.AreEqual(2, allTickets.Count);
        Assert.IsTrue(allTickets.Any(t => t.Id == ticket1.Id));
        Assert.IsTrue(allTickets.Any(t => t.Id == ticket2.Id));
    }

    [TestMethod]
    public void GetById_ShouldReturnTicket_WhenExists()
    {
        // Arrange
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);
        this.repository.Add(ticket);

        // Act
        var result = this.repository.GetById(ticket.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ticket.Id, result.Id);
        Assert.AreEqual(ticket.QrCode, result.QrCode);
    }

    [TestMethod]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = this.repository.GetById(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByQrCode_ShouldReturnTicket_WhenExists()
    {
        // Arrange
        var qrCode = Guid.NewGuid();

        // Create a visitor first since Ticket requires a valid VisitorId
        var visitor = new Visitor("Test", "Visitor", "test@example.com", "pass123",
            DateTime.UtcNow.AddYears(-20), Domain.Enums.MembershipLevel.Standard, Guid.NewGuid());
        this.context.Visitors.Add(visitor);
        this.context.SaveChanges();

        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: qrCode,
            specialEventId: null)
        {
            VisitorId = visitor.Id,
        };
        this.repository.Add(ticket);

        // Act
        var result = this.repository.GetByQrCode(qrCode);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ticket.Id, result.Id);
        Assert.AreEqual(qrCode, result.QrCode);
        Assert.IsNotNull(result.Visitor); // Verify the Visitor navigation property is loaded
        Assert.AreEqual(visitor.Id, result.Visitor.Id);
    }

    [TestMethod]
    public void GetByQrCode_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentQrCode = Guid.NewGuid();

        // Act
        var result = this.repository.GetByQrCode(nonExistentQrCode);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldModifyExistingTicket()
    {
        // Arrange
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);
        this.repository.Add(ticket);

        var newVisitDate = DateTime.UtcNow.AddDays(5);
        var newEventId = Guid.NewGuid();
        var updatedTicket = new Ticket(
            visitDate: newVisitDate,
            type: TicketType.SpecialEvent,
            qrCode: Guid.NewGuid(),
            specialEventId: newEventId);

        // Act
        var result = this.repository.Update(ticket.Id, updatedTicket);

        // Assert
        Assert.IsTrue(result);
        var updated = this.context.Tickets.Find(ticket.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual(newVisitDate, updated.VisitDate);
        Assert.AreEqual(TicketType.SpecialEvent, updated.Type);
        Assert.AreEqual(newEventId, updated.SpecialEventId);
    }

    [TestMethod]
    public void Update_ShouldReturnFalse_WhenTicketNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);

        // Act
        var result = this.repository.Update(nonExistentId, ticket);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Add_ShouldPersistEntity()
    {
        // Arrange
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);

        // Act
        this.repository.Add(ticket);

        // Assert
        var found = this.context.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(ticket.QrCode, found.QrCode);
    }

    [TestMethod]
    public void CountTicketsByEventId_ShouldReturnCorrectCount()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var ticket1 = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.SpecialEvent,
            qrCode: Guid.NewGuid(),
            specialEventId: eventId);
        var ticket2 = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(2),
            type: TicketType.SpecialEvent,
            qrCode: Guid.NewGuid(),
            specialEventId: eventId);
        var ticket3 = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(3),
            type: TicketType.SpecialEvent,
            qrCode: Guid.NewGuid(),
            specialEventId: Guid.NewGuid());

        this.repository.Add(ticket1);
        this.repository.Add(ticket2);
        this.repository.Add(ticket3);

        // Act
        var count = this.repository.CountTicketsByEventId(eventId);

        // Assert
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void CountTicketsByEventId_ShouldReturnZero_WhenNoTicketsForEvent()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var ticket = new Ticket(
            visitDate: DateTime.UtcNow.AddDays(1),
            type: TicketType.General,
            qrCode: Guid.NewGuid(),
            specialEventId: null);
        this.repository.Add(ticket);

        // Act
        var count = this.repository.CountTicketsByEventId(eventId);

        // Assert
        Assert.AreEqual(0, count);
    }
}
