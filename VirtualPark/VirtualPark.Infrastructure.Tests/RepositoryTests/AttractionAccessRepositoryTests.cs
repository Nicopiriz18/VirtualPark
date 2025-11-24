// <copyright file="AttractionAccessRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class AttractionAccessRepositoryTests
{
    private AttractionAccessRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        this.context = new ParkDbContext(options);
        this.repository = new AttractionAccessRepository(this.context);
    }

    [TestMethod]
    public void Add_ShouldAddNewAttractionAccess()
    {
        // Arrange
        var attractionAccess = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        // Act
        var createdAccess = this.repository.Add(attractionAccess);

        // Assert
        Assert.IsNotNull(createdAccess);
        Assert.AreEqual(attractionAccess.AttractionId, createdAccess.AttractionId);
        Assert.AreEqual(attractionAccess.VisitorId, createdAccess.VisitorId);
        Assert.AreEqual(attractionAccess.TicketId, createdAccess.TicketId);
        Assert.AreEqual(attractionAccess.EntryTime, createdAccess.EntryTime);
        Assert.IsNull(createdAccess.ExitTime);
        Assert.AreEqual(attractionAccess.EntryMethod, createdAccess.EntryMethod);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllAttractionAccessRecords()
    {
        // Arrange
        var access1 = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow.AddHours(-2),
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);
        var access2 = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow.AddHours(-1),
            exitTime: null,
            method: Domain.Enums.EntryMethod.NFC);

        this.repository.Add(access1);
        this.repository.Add(access2);

        // Act
        var allAccessRecords = this.repository.GetAll().ToList();

        // Assert
        Assert.AreEqual(2, allAccessRecords.Count);
        Assert.IsTrue(allAccessRecords.Any(a => a.Id == access1.Id));
        Assert.IsTrue(allAccessRecords.Any(a => a.Id == access2.Id));
    }

    [TestMethod]
    public void Add_ShouldPersistEntity()
    {
        // Arrange
        var access = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        // Act
        this.repository.Add(access);

        // Assert
        var found = this.context.AttractionAccesses.FirstOrDefault(a => a.Id == access.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(access.AttractionId, found.AttractionId);
    }

    [TestMethod]
    public void GetOpenAccess_ShouldReturnOpenAccessForVisitorAndAttraction()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var openAccess = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);
        var closedAccess = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow.AddHours(-1),
            exitTime: DateTime.UtcNow,
            method: Domain.Enums.EntryMethod.QR);

        this.repository.Add(openAccess);
        this.repository.Add(closedAccess);

        // Act
        var result = this.repository.GetOpenAccess(attractionId, visitorId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(openAccess.Id, result.Id);
    }

    [TestMethod]
    public void Update_ShouldModifyExistingEntity()
    {
        // Arrange
        var access = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);
        this.repository.Add(access);

        // Act
        access.ExitTime = DateTime.UtcNow.AddMinutes(10);
        this.repository.Update(access);

        // Assert
        var updated = this.context.AttractionAccesses.FirstOrDefault(a => a.Id == access.Id);
        Assert.IsNotNull(updated);
        Assert.IsNotNull(updated.ExitTime);
        Assert.AreEqual(access.ExitTime.Value, updated.ExitTime.Value);
    }

    [TestMethod]
    public void CountOpenAccesses_ShouldReturnCorrectCount()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var open1 = new AttractionAccess(
            attractionId: attractionId,
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);
        var open2 = new AttractionAccess(
            attractionId: attractionId,
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: Domain.Enums.EntryMethod.NFC);
        var closed = new AttractionAccess(
            attractionId: attractionId,
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: DateTime.UtcNow,
            exitTime: DateTime.UtcNow,
            method: Domain.Enums.EntryMethod.QR);

        this.repository.Add(open1);
        this.repository.Add(open2);
        this.repository.Add(closed);

        // Act
        var count = this.repository.CountOpenAccesses(attractionId);

        // Assert
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void GetOpenAccessByTicket_DevuelveAccesoAbierto()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var access = new AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), ticketId, DateTime.Now, null, Domain.Enums.EntryMethod.QR);
        this.context.AttractionAccesses.Add(access);
        this.context.SaveChanges();

        // Act
        var result = this.repository.GetOpenAccessByTicket(ticketId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ticketId, result.TicketId);
        Assert.IsNull(result.ExitTime);
    }

    [TestMethod]
    public void GetAccessesBetweenDates_ShouldReturnAccessesWithinDateRange()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        var accessInRange1 = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 1, 15),
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        var accessInRange2 = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 1, 20),
            exitTime: null,
            method: Domain.Enums.EntryMethod.NFC);

        var accessOutOfRange = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 2, 5),
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        this.repository.Add(accessInRange1);
        this.repository.Add(accessInRange2);
        this.repository.Add(accessOutOfRange);

        // Act
        var result = this.repository.GetAccessesBetweenDates(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(a => a.Id == accessInRange1.Id));
        Assert.IsTrue(result.Any(a => a.Id == accessInRange2.Id));
        Assert.IsFalse(result.Any(a => a.Id == accessOutOfRange.Id));
    }

    [TestMethod]
    public void GetAccessesBetweenDates_ShouldIncludeAccessesAtExactStartAndEndDates()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        var accessAtStart = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: startDate,
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        var accessAtEnd = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: endDate,
            exitTime: null,
            method: Domain.Enums.EntryMethod.NFC);

        this.repository.Add(accessAtStart);
        this.repository.Add(accessAtEnd);

        // Act
        var result = this.repository.GetAccessesBetweenDates(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(a => a.Id == accessAtStart.Id));
        Assert.IsTrue(result.Any(a => a.Id == accessAtEnd.Id));
    }

    [TestMethod]
    public void GetAccessesBetweenDates_ShouldReturnEmptyListWhenNoAccessesInRange()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        var accessBefore = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2023, 12, 15),
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        var accessAfter = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 2, 15),
            exitTime: null,
            method: Domain.Enums.EntryMethod.NFC);

        this.repository.Add(accessBefore);
        this.repository.Add(accessAfter);

        // Act
        var result = this.repository.GetAccessesBetweenDates(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAccessesBetweenDates_ShouldReturnEmptyListWhenNoAccessesExist()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        // Act
        var result = this.repository.GetAccessesBetweenDates(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAccessesBetweenDates_ShouldIncludeBothOpenAndClosedAccesses()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        var openAccess = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 1, 10),
            exitTime: null,
            method: Domain.Enums.EntryMethod.QR);

        var closedAccess = new AttractionAccess(
            attractionId: Guid.NewGuid(),
            visitorId: Guid.NewGuid(),
            ticketId: Guid.NewGuid(),
            entryTime: new DateTime(2024, 1, 15),
            exitTime: new DateTime(2024, 1, 16),
            method: Domain.Enums.EntryMethod.NFC);

        this.repository.Add(openAccess);
        this.repository.Add(closedAccess);

        // Act
        var result = this.repository.GetAccessesBetweenDates(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(a => a.Id == openAccess.Id && a.ExitTime == null));
        Assert.IsTrue(result.Any(a => a.Id == closedAccess.Id && a.ExitTime != null));
    }
}
