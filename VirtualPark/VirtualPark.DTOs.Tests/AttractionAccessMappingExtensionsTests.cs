// <copyright file="AttractionAccessMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.AttractionAccess;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class AttractionAccessMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapAttractionAccessToDtoCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var entryTime = new DateTime(2025, 10, 6, 10, 30, 0);
        var exitTime = new DateTime(2025, 10, 6, 11, 15, 0);
        var attractionAccess = new Domain.AttractionAccess(
            attractionId,
            visitorId,
            ticketId,
            entryTime,
            exitTime,
            EntryMethod.NFC);

        // Act
        var dto = attractionAccess.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attractionAccess.Id, dto.Id);
        Assert.AreEqual(attractionId, dto.AttractionId);
        Assert.AreEqual(visitorId, dto.VisitorId);
        Assert.AreEqual(ticketId, dto.TicketId);
        Assert.AreEqual(entryTime, dto.EntryTime);
        Assert.AreEqual(exitTime, dto.ExitTime);
        Assert.AreEqual(EntryMethod.NFC, dto.EntryMethod);
        Assert.IsTrue(dto.IsClosed);
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapAttractionAccessToDetailDtoWithDuration()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var entryTime = new DateTime(2025, 10, 6, 14, 0, 0);
        var exitTime = new DateTime(2025, 10, 6, 15, 30, 0);
        var attractionAccess = new Domain.AttractionAccess(
            attractionId,
            visitorId,
            ticketId,
            entryTime,
            exitTime,
            EntryMethod.QR);

        // Act
        var dto = attractionAccess.ToDetailDto("Roller Coaster", "John Doe");

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attractionAccess.Id, dto.Id);
        Assert.AreEqual("Roller Coaster", dto.AttractionName);
        Assert.AreEqual("John Doe", dto.VisitorName);
        Assert.IsNotNull(dto.Duration);
        Assert.AreEqual(TimeSpan.FromMinutes(90), dto.Duration);
    }

    [TestMethod]
    public void ToDetailDto_ShouldHandleNullExitTime()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var entryTime = DateTime.Now;
        var attractionAccess = new Domain.AttractionAccess(
            attractionId,
            visitorId,
            ticketId,
            entryTime,
            null,
            EntryMethod.NFC);

        // Act
        var dto = attractionAccess.ToDetailDto("Water Slide", "Jane Smith");

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Water Slide", dto.AttractionName);
        Assert.AreEqual("Jane Smith", dto.VisitorName);
        Assert.IsNull(dto.ExitTime);
        Assert.IsNull(dto.Duration);
        Assert.IsFalse(dto.IsClosed);
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfAttractionAccessesToListOfDtos()
    {
        // Arrange
        var accesses = new List<Domain.AttractionAccess>
        {
            new Domain.AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddHours(-2), null, EntryMethod.NFC),
            new Domain.AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddHours(-1), null, EntryMethod.QR),
        };

        // Act
        var dtos = accesses.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual(EntryMethod.NFC, dtos[0].EntryMethod);
        Assert.AreEqual(EntryMethod.QR, dtos[1].EntryMethod);
    }

    [TestMethod]
    public void ToDetailDto_Collection_ShouldMapListOfAttractionAccessesToListOfDetailDtos()
    {
        // Arrange
        var accesses = new List<Domain.AttractionAccess>
        {
            new Domain.AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddHours(-2), null, EntryMethod.NFC),
            new Domain.AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddHours(-1), null, EntryMethod.QR),
        };

        Func<Domain.AttractionAccess, string> getAttractionName = aa => "Attraction";
        Func<Domain.AttractionAccess, string> getVisitorName = aa => "Visitor";

        // Act
        var dtos = accesses.ToDetailDto(getAttractionName, getVisitorName);

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Attraction", dtos[0].AttractionName);
        Assert.AreEqual("Visitor", dtos[0].VisitorName);
    }

    [TestMethod]
    public void ToDto_ShouldHandleOpenAccess()
    {
        // Arrange
        var attractionAccess = new Domain.AttractionAccess(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            null,
            EntryMethod.NFC);

        // Act
        var dto = attractionAccess.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsNull(dto.ExitTime);
        Assert.IsFalse(dto.IsClosed);
    }
}
