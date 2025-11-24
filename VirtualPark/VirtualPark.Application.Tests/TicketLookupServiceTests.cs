// <copyright file="TicketLookupServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class TicketLookupServiceTests
{
    private Mock<ITicketRepository> mockTicketRepo = null!;
    private Mock<IVisitorRepository> mockVisitorRepo = null!;
    private TicketLookupService service = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockTicketRepo = new Mock<ITicketRepository>(MockBehavior.Strict);
        this.mockVisitorRepo = new Mock<IVisitorRepository>(MockBehavior.Strict);
        this.service = new TicketLookupService(this.mockTicketRepo.Object, this.mockVisitorRepo.Object);
    }

    [TestMethod]
    public void GetTicketByQrCode_ReturnsTicketWithVisitor_WhenFound()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("John", "Doe", "john@example.com", "password", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var ticket = new Ticket(DateTime.Today, TicketType.General, qrCode) { VisitorId = visitorId };

        this.mockTicketRepo.Setup(r => r.GetByQrCode(qrCode)).Returns(ticket);
        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);

        // Act
        var result = this.service.GetTicketByQrCode(qrCode);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(qrCode, result.QrCode);
        Assert.AreEqual(visitor, result.Visitor);
        this.mockTicketRepo.VerifyAll();
        this.mockVisitorRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetTicketByQrCode_ThrowsException_WhenTicketNotFound()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        this.mockTicketRepo.Setup(r => r.GetByQrCode(qrCode)).Returns((Ticket)null!);

        // Act
        this.service.GetTicketByQrCode(qrCode);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetTicketByQrCode_ThrowsException_WhenVisitorNotFound()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticket = new Ticket(DateTime.Today, TicketType.General, qrCode) { VisitorId = visitorId };

        this.mockTicketRepo.Setup(r => r.GetByQrCode(qrCode)).Returns(ticket);
        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns((Visitor)null!);

        // Act
        this.service.GetTicketByQrCode(qrCode);
    }

    [TestMethod]
    public void GetTicketByVisitorAndDate_ReturnsTicketWithVisitor_WhenFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);
        var visitor = new Visitor("Jane", "Smith", "jane@example.com", "password", new DateTime(1995, 5, 15), MembershipLevel.Premium, Guid.NewGuid());
        var ticket = new Ticket(visitDate, TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);
        this.mockTicketRepo.Setup(r => r.GetAll()).Returns(new List<Ticket> { ticket });

        // Act
        var result = this.service.GetTicketByVisitorAndDate(visitorId, visitDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(visitorId, result.VisitorId);
        Assert.AreEqual(visitor, result.Visitor);
        this.mockVisitorRepo.VerifyAll();
        this.mockTicketRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetTicketByVisitorAndDate_ThrowsException_WhenVisitorNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns((Visitor)null!);

        // Act
        this.service.GetTicketByVisitorAndDate(visitorId, visitDate);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetTicketByVisitorAndDate_ThrowsException_WhenNoTicketFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);
        var visitor = new Visitor("Bob", "Johnson", "bob@example.com", "password", new DateTime(1992, 3, 20), MembershipLevel.VIP, Guid.NewGuid());

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);
        this.mockTicketRepo.Setup(r => r.GetAll()).Returns(new List<Ticket>());

        // Act
        this.service.GetTicketByVisitorAndDate(visitorId, visitDate);
    }

    [TestMethod]
    public void GetTicketByVisitorAndDate_ReturnsMostRecentTicket_WhenMultipleTicketsExist()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);
        var visitor = new Visitor("Alice", "Wonder", "alice@example.com", "password", new DateTime(1988, 7, 10), MembershipLevel.Standard, Guid.NewGuid());

        var ticket1 = new Ticket(visitDate.AddHours(10), TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };
        var ticket2 = new Ticket(visitDate.AddHours(14), TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };
        var ticket3 = new Ticket(visitDate.AddHours(8), TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);
        this.mockTicketRepo.Setup(r => r.GetAll()).Returns(new List<Ticket> { ticket1, ticket2, ticket3 });

        // Act
        var result = this.service.GetTicketByVisitorAndDate(visitorId, visitDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ticket2.QrCode, result.QrCode); // Should be the most recent (14:00)
        Assert.AreEqual(visitor, result.Visitor);
        this.mockVisitorRepo.VerifyAll();
        this.mockTicketRepo.VerifyAll();
    }

    [TestMethod]
    public void GetTicketByVisitorAndDate_IgnoresTicketsFromOtherDates()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);
        var visitor = new Visitor("Charlie", "Brown", "charlie@example.com", "password", new DateTime(2000, 12, 25), MembershipLevel.Standard, Guid.NewGuid());

        var ticketToday = new Ticket(visitDate, TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };
        var ticketYesterday = new Ticket(visitDate.AddDays(-1), TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };
        var ticketTomorrow = new Ticket(visitDate.AddDays(1), TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);
        this.mockTicketRepo.Setup(r => r.GetAll()).Returns(new List<Ticket> { ticketToday, ticketYesterday, ticketTomorrow });

        // Act
        var result = this.service.GetTicketByVisitorAndDate(visitorId, visitDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ticketToday.QrCode, result.QrCode);
        this.mockVisitorRepo.VerifyAll();
        this.mockTicketRepo.VerifyAll();
    }

    [TestMethod]
    public void GetTicketByVisitorAndDate_IgnoresTicketsFromOtherVisitors()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var otherVisitorId = Guid.NewGuid();
        var visitDate = new DateTime(2025, 10, 16);
        var visitor = new Visitor("David", "Lee", "david@example.com", "password", new DateTime(1985, 4, 3), MembershipLevel.Premium, Guid.NewGuid());

        var myTicket = new Ticket(visitDate, TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };
        var otherTicket = new Ticket(visitDate, TicketType.General, Guid.NewGuid()) { VisitorId = otherVisitorId };

        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns(visitor);
        this.mockTicketRepo.Setup(r => r.GetAll()).Returns(new List<Ticket> { myTicket, otherTicket });

        // Act
        var result = this.service.GetTicketByVisitorAndDate(visitorId, visitDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(myTicket.QrCode, result.QrCode);
        this.mockVisitorRepo.VerifyAll();
        this.mockTicketRepo.VerifyAll();
    }
}
