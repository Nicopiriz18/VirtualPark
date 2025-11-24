// <copyright file="VisitorServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Visitors;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Tests;

[TestClass]
public class VisitorServiceTests
{
    private Mock<IVisitorRepository> mockVisitorRepository = null!;
    private Mock<ITicketRepository> mockTicketRepository = null!;
    private Mock<IPasswordHasher> mockPasswordHasher = null!;
    private Mock<ISpecialEventRepository> mockSpecialEventRepository = null!;
    private VisitorService visitorService = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockVisitorRepository = new Mock<IVisitorRepository>();
        this.mockTicketRepository = new Mock<ITicketRepository>();
        this.mockPasswordHasher = new Mock<IPasswordHasher>();
        this.mockSpecialEventRepository = new Mock<ISpecialEventRepository>();
        this.visitorService = new VisitorService(
            this.mockVisitorRepository.Object,
            this.mockPasswordHasher.Object);
    }

    [TestMethod]
    public void RegisterUser_WithValidData_ShouldCreateUserWithStandardMembership()
    {
        // Arrange
        var name = "John";
        var surname = "Doe";
        var email = "john.doe@example.com";
        var password = "password123";
        var birthDate = new DateTime(1990, 1, 1);
        var nfcId = Guid.NewGuid();

        this.mockVisitorRepository
            .Setup(r => r.Add(It.IsAny<Visitor>()))
            .Returns((Visitor v) => v);

        // Act
        var visitor = this.visitorService.RegisterUser(name, surname, email, password, birthDate, nfcId);

        // Assert
        Assert.IsNotNull(visitor);
        Assert.AreEqual(name, visitor.Name);
        Assert.AreEqual(surname, visitor.Surname);
        Assert.AreEqual(email, visitor.Email);
        Assert.AreEqual(MembershipLevel.Standard, visitor.MembershipLevel);
        Assert.AreEqual(birthDate, visitor.BirthDate);
        Assert.AreEqual(nfcId, visitor.NfcId);
        this.mockVisitorRepository.Verify(r => r.Add(It.IsAny<Visitor>()), Times.Once);
    }

    [TestMethod]
    public void RegisterUserByAdmin_WithValidData_ShouldCreateUserWithSpecifiedMembership()
    {
        // Arrange
        var name = "Jane";
        var surname = "Smith";
        var email = "jane.smith@example.com";
        var password = "password456";
        var birthDate = new DateTime(1985, 5, 15);
        var nfcId = Guid.NewGuid();
        var membershipLevel = MembershipLevel.Premium;

        this.mockVisitorRepository
            .Setup(r => r.Add(It.IsAny<Visitor>()))
            .Returns((Visitor v) => v);

        // Act
        var visitor = this.visitorService.RegisterUserByAdmin(name, surname, email, password, birthDate, nfcId, membershipLevel);

        // Assert
        Assert.IsNotNull(visitor);
        Assert.AreEqual(name, visitor.Name);
        Assert.AreEqual(surname, visitor.Surname);
        Assert.AreEqual(email, visitor.Email);
        Assert.AreEqual(membershipLevel, visitor.MembershipLevel);
        Assert.AreEqual(birthDate, visitor.BirthDate);
        Assert.AreEqual(nfcId, visitor.NfcId);
        this.mockVisitorRepository.Verify(r => r.Add(It.IsAny<Visitor>()), Times.Once);
    }

    [TestMethod]
    public void UpdateProfile_WithValidData_ShouldUpdateUserInfo()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var existingVisitor = new Visitor("John", "Doe", "john@example.com", "pass123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var newName = "John Updated";
        var newSurname = "Doe Updated";
        var newEmail = "john.updated@example.com";

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.IsAny<Visitor>()))
            .Returns(true);

        // Act
        this.visitorService.UpdateProfile(visitorId, newName, newSurname, newEmail);

        // Assert
        this.mockVisitorRepository.Verify(r => r.GetById(visitorId), Times.Once);
        this.mockVisitorRepository.Verify(
            r => r.Update(visitorId, It.Is<Visitor>(v =>
            v.Name == newName && v.Surname == newSurname && v.Email == newEmail)), Times.Once);
    }

    [TestMethod]
    public void UpdateProfileWithPassword_WithNewPassword_ShouldHashAndUpdatePassword()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var existingVisitor = new Visitor("John", "Doe", "john@example.com", "oldHashedPass", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var newName = "John Updated";
        var newSurname = "Doe Updated";
        var newEmail = "john.updated@example.com";
        var newPassword = "newPassword123";
        var hashedPassword = "hashedNewPassword123";

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockPasswordHasher
            .Setup(h => h.Hash(newPassword))
            .Returns(hashedPassword);

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.IsAny<Visitor>()))
            .Returns(true);

        // Act
        this.visitorService.UpdateProfileWithPassword(visitorId, newName, newSurname, newEmail, newPassword);

        // Assert
        this.mockVisitorRepository.Verify(r => r.GetById(visitorId), Times.Once);
        this.mockPasswordHasher.Verify(h => h.Hash(newPassword), Times.Once);
        this.mockVisitorRepository.Verify(
            r => r.Update(visitorId, It.Is<Visitor>(v =>
            v.Name == newName && v.Surname == newSurname && v.Email == newEmail && v.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void UpdateProfileWithPassword_WithNullPassword_ShouldKeepExistingPassword()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var oldPassword = "oldHashedPass";
        var existingVisitor = new Visitor("John", "Doe", "john@example.com", oldPassword, new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var newName = "John Updated";
        var newSurname = "Doe Updated";
        var newEmail = "john.updated@example.com";

        this.mockVisitorRepository
            .Setup(r => r.GetById(visitorId))
            .Returns(existingVisitor);

        this.mockVisitorRepository
            .Setup(r => r.Update(visitorId, It.IsAny<Visitor>()))
            .Returns(true);

        // Act
        this.visitorService.UpdateProfileWithPassword(visitorId, newName, newSurname, newEmail, null);

        // Assert
        this.mockVisitorRepository.Verify(r => r.GetById(visitorId), Times.Once);
        this.mockPasswordHasher.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        this.mockVisitorRepository.Verify(
            r => r.Update(visitorId, It.Is<Visitor>(v =>
            v.Name == newName && v.Surname == newSurname && v.Email == newEmail && v.Password == oldPassword)), Times.Once);
    }

    // [TestMethod]
    // public void PurchaseTicket_WithValidData_ShouldCreateGeneralTicket()
    // {
    //     // Arrange
    //     var visitorId = Guid.NewGuid();
    //     var visitor = new Visitor("John", "Doe", "john@example.com", "pass123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
    //     var visitDate = DateTime.Today.AddDays(5);
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetById(visitorId))
    //         .Returns(visitor);
    //
    //     _mockTicketRepository
    //         .Setup(r => r.Add(It.IsAny<Ticket>()))
    //         .Returns((Ticket t) => t);
    //
    //     // Act
    //     var ticket = _visitorService.PurchaseTicket(visitorId, visitDate, TicketType.General);
    //
    //     // Assert
    //     Assert.IsNotNull(ticket);
    //     Assert.AreEqual(visitDate, ticket.VisitDate);
    //     Assert.AreEqual(TicketType.General, ticket.Type);
    //     Assert.AreNotEqual(Guid.Empty, ticket.QrCode);
    //     Assert.IsNull(ticket.SpecialEventId);
    //     Assert.AreEqual(visitorId, ticket.VisitorId);
    //     _mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Once);
    // }
    //
    // [TestMethod]
    // public void PurchaseSpecialEventTicket_WithValidData_ShouldCreateSpecialEventTicket()
    // {
    //     // Arrange
    //     var visitorId = Guid.NewGuid();
    //     var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15), MembershipLevel.Standard, Guid.NewGuid());
    //     var visitDate = DateTime.Today.AddDays(10);
    //     var specialEventId = Guid.NewGuid();
    //     var specialEvent = new Domain.SpecialEvent("Concert", DateTime.Today.AddDays(10), 100, 50.0m);
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetById(visitorId))
    //         .Returns(visitor);
    //
    //     _mockSpecialEventRepository
    //         .Setup(r => r.GetById(specialEventId))
    //         .Returns(specialEvent);
    //
    //     _mockTicketRepository
    //         .Setup(r => r.CountTicketsByEventId(specialEventId))
    //         .Returns(50); // 50 tickets already sold
    //
    //     _mockTicketRepository
    //         .Setup(r => r.Add(It.IsAny<Ticket>()))
    //         .Returns((Ticket t) => t);
    //
    //     // Act
    //     var ticket = _visitorService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId);
    //
    //     // Assert
    //     Assert.IsNotNull(ticket);
    //     Assert.AreEqual(visitDate, ticket.VisitDate);
    //     Assert.AreEqual(TicketType.SpecialEvent, ticket.Type);
    //     Assert.AreNotEqual(Guid.Empty, ticket.QrCode);
    //     Assert.AreEqual(specialEventId, ticket.SpecialEventId);
    //     Assert.AreEqual(visitorId, ticket.VisitorId);
    //     _mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Once);
    //     _mockTicketRepository.Verify(r => r.CountTicketsByEventId(specialEventId), Times.Once);
    // }
    //
    // [TestMethod]
    // public void PurchaseSpecialEventTicket_WhenAtCapacity_ShouldThrowInvalidOperationException()
    // {
    //     // Arrange
    //     var visitorId = Guid.NewGuid();
    //     var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15), MembershipLevel.Standard, Guid.NewGuid());
    //     var visitDate = DateTime.Today.AddDays(10);
    //     var specialEventId = Guid.NewGuid();
    //     var specialEvent = new Domain.SpecialEvent("Concert", DateTime.Today.AddDays(10), 100, 50.0m);
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetById(visitorId))
    //         .Returns(visitor);
    //
    //     _mockSpecialEventRepository
    //         .Setup(r => r.GetById(specialEventId))
    //         .Returns(specialEvent);
    //
    //     _mockTicketRepository
    //         .Setup(r => r.CountTicketsByEventId(specialEventId))
    //         .Returns(100); // Event is at full capacity
    //
    //     // Act & Assert
    //     var exception = Assert.ThrowsException<InvalidOperationException>(() =>
    //         _visitorService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));
    //
    //     Assert.IsTrue(exception.Message.Contains("maximum capacity"));
    //     _mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    // }
    //
    // [TestMethod]
    // public void PurchaseSpecialEventTicket_WhenEventNotFound_ShouldThrowArgumentException()
    // {
    //     // Arrange
    //     var visitorId = Guid.NewGuid();
    //     var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15), MembershipLevel.Standard, Guid.NewGuid());
    //     var visitDate = DateTime.Today.AddDays(10);
    //     var specialEventId = Guid.NewGuid();
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetById(visitorId))
    //         .Returns(visitor);
    //
    //     _mockSpecialEventRepository
    //         .Setup(r => r.GetById(specialEventId))
    //         .Returns((Domain.SpecialEvent?)null);
    //
    //     // Act & Assert
    //     var exception = Assert.ThrowsException<ArgumentException>(() =>
    //         _visitorService.PurchaseSpecialEventTicket(visitorId, visitDate, specialEventId));
    //
    //     Assert.IsTrue(exception.Message.Contains("Special event not found"));
    //     _mockTicketRepository.Verify(r => r.Add(It.IsAny<Ticket>()), Times.Never);
    // }

    // [TestMethod]
    // public void RegisterAttractionVisit_WithNFC_ShouldCreateAttractionAccess()
    // {
    //     // Arrange
    //     var nfcId = Guid.NewGuid();
    //     var visitor = new Visitor("John", "Doe", "john@example.com", "pass123", new DateTime(1990, 1, 1), MembershipLevel.Standard, nfcId);
    //     var attractionId = Guid.NewGuid();
    //     var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid(), null) { VisitorId = visitor.Id };
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetByNfcId(nfcId))
    //         .Returns(visitor);
    //
    //     // Act
    //     var access = _visitorService.RegisterAttractionVisit(nfcId, attractionId, EntryMethod.NFC, ticket);
    //
    //     // Assert
    //     Assert.IsNotNull(access);
    //     Assert.AreEqual(visitor.Id, access.VisitorId);
    //     Assert.AreEqual(attractionId, access.AttractionId);
    //     Assert.AreEqual(EntryMethod.NFC, access.EntryMethod);
    //     Assert.IsTrue(access.EntryTime <= DateTime.Now);
    // }

    // [TestMethod]
    // public void RegisterAttractionVisit_WithQR_ShouldCreateAttractionAccess()
    // {
    //     // Arrange
    //     var visitor = new Visitor("Jane", "Smith", "jane@example.com", "pass456", new DateTime(1985, 5, 15), MembershipLevel.Standard, Guid.NewGuid());
    //     var attractionId = Guid.NewGuid();
    //     var qrCode = Guid.NewGuid();
    //     var ticket = new Ticket(DateTime.Today, TicketType.General, qrCode, null) { VisitorId = visitor.Id };
    //
    //     _mockVisitorRepository
    //         .Setup(r => r.GetById(visitor.Id))
    //         .Returns(visitor);
    //
    //     // Act
    //     var access = _visitorService.RegisterAttractionVisit(qrCode, attractionId, EntryMethod.QR, ticket);
    //
    //     // Assert
    //     Assert.IsNotNull(access);
    //     Assert.AreEqual(visitor.Id, access.VisitorId);
    //     Assert.AreEqual(attractionId, access.AttractionId);
    //     Assert.AreEqual(EntryMethod.QR, access.EntryMethod);
    //     Assert.IsTrue(access.EntryTime <= DateTime.Now);
    // }

    // TODO: We should test for the capacity of the special event when buying the ticket
}
