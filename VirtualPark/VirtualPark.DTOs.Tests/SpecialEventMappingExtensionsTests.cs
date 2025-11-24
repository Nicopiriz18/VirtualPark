// <copyright file="SpecialEventMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.SpecialEvents;
using VirtualPark.DTOs.SpecialEvents.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class SpecialEventMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapSpecialEventToDtoCorrectly()
    {
        // Arrange
        var date = new DateTime(2025, 12, 25);
        var specialEvent = new SpecialEvent(
            "Christmas Celebration",
            date,
            1000,
            75.5m);

        // Act
        var dto = specialEvent.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(specialEvent.Id, dto.Id);
        Assert.AreEqual("Christmas Celebration", dto.Name);
        Assert.AreEqual(date, dto.Date);
        Assert.AreEqual(1000, dto.MaxCapacity);
        Assert.AreEqual(75.5m, dto.AdditionalCost);
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapSpecialEventToDetailDtoWithAttractions()
    {
        // Arrange
        var date = new DateTime(2026, 10, 31);
        var specialEvent = new SpecialEvent("Halloween Event", date, 500, 50m);
        var attraction1 = new Attraction("Haunted House", "Scary ride", "Extreme", 13, 30);
        var attraction2 = new Attraction("Ghost Train", "Spooky train", "Family", 8, 40);
        specialEvent.AddAttraction(attraction1);
        specialEvent.AddAttraction(attraction2);

        // Act
        var dto = specialEvent.ToDetailDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(specialEvent.Id, dto.Id);
        Assert.AreEqual("Halloween Event", dto.Name);
        Assert.AreEqual(2, dto.TotalAttractions);
        Assert.AreEqual(2, dto.Attractions.Count);
        Assert.AreEqual("Haunted House", dto.Attractions[0].Name);
        Assert.AreEqual("Ghost Train", dto.Attractions[1].Name);
    }

    [TestMethod]
    public void ToCapacityDto_ShouldMapSpecialEventToCapacityDtoCorrectly()
    {
        // Arrange
        var specialEvent = new SpecialEvent("Summer Festival", DateTime.Now.AddDays(60), 1000, 100m);
        var currentTicketsSold = 750;
        var requestedTickets = 200;

        // Act
        var dto = specialEvent.ToCapacityDto(currentTicketsSold, requestedTickets);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(specialEvent.Id, dto.EventId);
        Assert.AreEqual("Summer Festival", dto.EventName);
        Assert.AreEqual(1000, dto.MaxCapacity);
        Assert.AreEqual(750, dto.CurrentTicketsSold);
        Assert.AreEqual(250, dto.AvailableCapacity);
        Assert.AreEqual(200, dto.RequestedTickets);
        Assert.IsTrue(dto.HasCapacity);
    }

    [TestMethod]
    public void ToCapacityDto_ShouldIndicateNoCapacityWhenFull()
    {
        // Arrange
        var specialEvent = new SpecialEvent("New Year's Eve", new DateTime(2025, 12, 31), 500, 200m);
        var currentTicketsSold = 490;
        var requestedTickets = 20;

        // Act
        var dto = specialEvent.ToCapacityDto(currentTicketsSold, requestedTickets);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(10, dto.AvailableCapacity);
        Assert.IsFalse(dto.HasCapacity);
    }

    [TestMethod]
    public void ToCapacityDto_ShouldHandleExactCapacity()
    {
        // Arrange
        var specialEvent = new SpecialEvent("Spring Festival", DateTime.Today.AddDays(90), 300, 30m);
        var currentTicketsSold = 250;
        var requestedTickets = 50;

        // Act
        var dto = specialEvent.ToCapacityDto(currentTicketsSold, requestedTickets);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(50, dto.AvailableCapacity);
        Assert.IsTrue(dto.HasCapacity);
    }

    [TestMethod]
    public void ToDomain_ShouldMapCreateSpecialEventRequestDtoToSpecialEvent()
    {
        // Arrange
        var date = DateTime.Today.AddDays(60);
        var dto = new CreateSpecialEventRequestDto
        {
            Name = "Summer Concert",
            Date = date,
            MaxCapacity = 2000,
            AdditionalCost = 150m,
        };

        // Act
        var specialEvent = dto.ToDomain();

        // Assert
        Assert.IsNotNull(specialEvent);
        Assert.AreEqual("Summer Concert", specialEvent.Name);
        Assert.AreEqual(date, specialEvent.Date);
        Assert.AreEqual(2000, specialEvent.MaxCapacity);
        Assert.AreEqual(150m, specialEvent.AdditionalCost);
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfSpecialEventsToListOfDtos()
    {
        // Arrange
        var events = new List<SpecialEvent>
        {
            new SpecialEvent("Event1", DateTime.Now.AddDays(10), 500, 50m),
            new SpecialEvent("Event2", DateTime.Now.AddDays(20), 1000, 75m),
        };

        // Act
        var dtos = events.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Event1", dtos[0].Name);
        Assert.AreEqual("Event2", dtos[1].Name);
    }

    [TestMethod]
    public void ToDetailDto_Collection_ShouldMapListOfSpecialEventsToListOfDetailDtos()
    {
        // Arrange
        var events = new List<SpecialEvent>
        {
            new SpecialEvent("Event1", DateTime.Now.AddDays(10), 500, 50m),
            new SpecialEvent("Event2", DateTime.Now.AddDays(20), 1000, 75m),
            new SpecialEvent("Event3", DateTime.Now.AddDays(30), 750, 60m),
        };

        // Act
        var dtos = events.ToDetailDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(3, dtos.Count);
        Assert.AreEqual("Event1", dtos[0].Name);
        Assert.AreEqual("Event2", dtos[1].Name);
        Assert.AreEqual("Event3", dtos[2].Name);
    }
}
