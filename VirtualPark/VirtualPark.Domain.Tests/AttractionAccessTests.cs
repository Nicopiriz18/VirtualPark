// <copyright file="AttractionAccessTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reflection;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class AttractionAccessTests
{
    [TestMethod]
    public void AttactionAccessTestAll()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();

        // Act
        var acceso = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: ticketId,
            entryTime: now,
            exitTime: null,
            method: EntryMethod.QR);

        // Assert
        Assert.AreEqual(attractionId, acceso.AttractionId);
        Assert.AreEqual(visitorId, acceso.VisitorId);
        Assert.AreEqual(ticketId, acceso.TicketId);
        Assert.AreEqual(EntryMethod.QR, acceso.EntryMethod);
        Assert.AreEqual(now, acceso.EntryTime);
        Assert.IsNull(acceso.ExitTime);
        Assert.IsFalse(acceso.IsClosed);
    }

    [TestMethod]
    public void AttractionAccess_WhenDischargeTimeIsSet_IsClosedReturnsTrue()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dischargeTime = now.AddHours(2);
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();

        // Act
        var acceso = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: ticketId,
            entryTime: now,
            exitTime: dischargeTime,
            method: EntryMethod.QR);

        // Assert
        Assert.AreEqual(attractionId, acceso.AttractionId);
        Assert.AreEqual(visitorId, acceso.VisitorId);
        Assert.AreEqual(ticketId, acceso.TicketId);
        Assert.AreEqual(dischargeTime, acceso.ExitTime);
        Assert.IsTrue(acceso.IsClosed);
    }

    [TestMethod]
    public void AttractionAccess_IdProperty_CanBeSetAndRetrieved()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var id = Guid.NewGuid();

        // Act
        var acceso = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: ticketId,
            entryTime: DateTime.UtcNow,
            exitTime: null,
            method: EntryMethod.QR);

        acceso.Id = id;

        // Assert
        Assert.AreEqual(id, acceso.Id);
    }

    [TestMethod]
    public void AttractionAccess_WithNullTicketId_WorksCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();

        // Act
        var acceso = new AttractionAccess(
            attractionId: attractionId,
            visitorId: visitorId,
            ticketId: null,
            entryTime: now,
            exitTime: null,
            method: EntryMethod.QR);

        // Assert
        Assert.AreEqual(attractionId, acceso.AttractionId);
        Assert.AreEqual(visitorId, acceso.VisitorId);
        Assert.IsNull(acceso.TicketId);
        Assert.AreEqual(now, acceso.EntryTime);
        Assert.IsNull(acceso.ExitTime);
        Assert.IsFalse(acceso.IsClosed);
    }

    [TestMethod]
    public void AttractionAccess_PrivateConstructor_CanBeInvoked()
    {
        // Arrange
        var type = typeof(AttractionAccess);
        var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

        // Act
        var instance = constructor?.Invoke(null) as AttractionAccess;

        // Assert
        Assert.IsNotNull(instance);
        Assert.AreEqual(Guid.Empty, instance.Id);
        Assert.AreEqual(Guid.Empty, instance.AttractionId);
        Assert.AreEqual(Guid.Empty, instance.VisitorId);
        Assert.IsNull(instance.TicketId);
        Assert.AreEqual(DateTime.MinValue, instance.EntryTime);
        Assert.IsNull(instance.ExitTime);
        Assert.AreEqual(EntryMethod.QR, instance.EntryMethod);
        Assert.IsFalse(instance.IsClosed);
    }

    [TestMethod]
    public void MarkExit_SetsExitTime_WhenNotSet()
    {
        // Arrange
        var acceso = new AttractionAccess(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow, null, EntryMethod.QR);

        var salida = DateTime.UtcNow.AddHours(1);

        // Act
        acceso.MarkExit(salida);

        // Assert
        Assert.AreEqual(salida, acceso.ExitTime);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MarkExit_Throws_WhenExitTimeAlreadySet()
    {
        // Arrange
        var acceso = new AttractionAccess(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow, DateTime.UtcNow.AddHours(1), EntryMethod.QR);

        // Act
        acceso.MarkExit(DateTime.UtcNow.AddHours(2));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MarkExit_Throws_WhenExitTimeIsBeforeEntryTime()
    {
        // Arrange
        var entryTime = DateTime.UtcNow;
        var exitTime = entryTime.AddMinutes(-10); // salida antes de la entrada
        var acceso = new AttractionAccess(
            Guid.NewGuid(), Guid.NewGuid(), null,
            entryTime, null, EntryMethod.QR);

        // Act
        acceso.MarkExit(exitTime);
    }
}
