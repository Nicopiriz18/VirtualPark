// <copyright file="TicketMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Tickets;
using VirtualPark.DTOs.Tickets.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class TicketMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapTicketToDtoCorrectly()
    {
        // Arrange
        var visitDate = new DateTime(2025, 10, 15);
        var qrCode = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();
        var ticket = new Ticket(visitDate, TicketType.SpecialEvent, qrCode, specialEventId);

        // Act
        var dto = ticket.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(ticket.Id, dto.Id);
        Assert.AreEqual(ticket.VisitorId, dto.VisitorId);
        Assert.AreEqual(visitDate, dto.VisitDate);
        Assert.AreEqual(TicketType.SpecialEvent, dto.Type);
        Assert.AreEqual(qrCode, dto.QrCode);
        Assert.AreEqual(specialEventId, dto.SpecialEventId);
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapTicketToDetailDtoCorrectly()
    {
        // Arrange
        var visitDate = new DateTime(2025, 10, 15);
        var qrCode = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();
        var ticket = new Ticket(visitDate, TicketType.SpecialEvent, qrCode, specialEventId);
        var visitorName = "John Doe";
        var eventName = "Summer Concert";
        var eventCost = 150m;

        // Act
        var dto = ticket.ToDetailDto(visitorName, eventName, eventCost);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(ticket.Id, dto.Id);
        Assert.AreEqual(ticket.VisitorId, dto.VisitorId);
        Assert.AreEqual("John Doe", dto.VisitorName);
        Assert.AreEqual("Summer Concert", dto.SpecialEventName);
        Assert.AreEqual(150m, dto.SpecialEventCost);
    }

    [TestMethod]
    public void ToDetailDto_ShouldHandleNullSpecialEvent()
    {
        // Arrange
        var visitDate = new DateTime(2025, 10, 15);
        var qrCode = Guid.NewGuid();
        var ticket = new Ticket(visitDate, TicketType.General, qrCode, null);
        var visitorName = "Jane Smith";

        // Act
        var dto = ticket.ToDetailDto(visitorName);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Jane Smith", dto.VisitorName);
        Assert.IsNull(dto.SpecialEventName);
        Assert.IsNull(dto.SpecialEventCost);
    }

    [TestMethod]
    public void ToDomain_ShouldMapPurchaseTicketRequestDtoToTicket()
    {
        // Arrange
        var visitDate = new DateTime(2025, 11, 1);
        var specialEventId = Guid.NewGuid();
        var qrCode = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var dto = new PurchaseTicketRequestDto
        {
            VisitorId = visitorId,
            VisitDate = visitDate,
            Type = TicketType.SpecialEvent,
            SpecialEventId = specialEventId,
        };

        // Act
        var ticket = dto.ToDomain(qrCode);

        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(visitDate, ticket.VisitDate);
        Assert.AreEqual(TicketType.SpecialEvent, ticket.Type);
        Assert.AreEqual(qrCode, ticket.QrCode);
        Assert.AreEqual(specialEventId, ticket.SpecialEventId);
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfTicketsToListOfDtos()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(DateTime.Now.AddDays(1), TicketType.General, Guid.NewGuid(), null),
            new Ticket(DateTime.Now.AddDays(2), TicketType.SpecialEvent, Guid.NewGuid(), Guid.NewGuid()),
        };

        // Act
        var dtos = tickets.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual(TicketType.General, dtos[0].Type);
        Assert.AreEqual(TicketType.SpecialEvent, dtos[1].Type);
    }
}
