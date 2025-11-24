// <copyright file="SpecialEventServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.SpecialEvent;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class SpecialEventServiceTests
{
    private Mock<ISpecialEventRepository> repoMock = null!;
    private Mock<ITicketRepository> ticketRepoMock = null!;
    private SpecialEventService service = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repoMock = new Mock<ISpecialEventRepository>(MockBehavior.Strict);
        this.ticketRepoMock = new Mock<ITicketRepository>(MockBehavior.Strict);
        var attractionRepoMock = new Mock<IAttractionRepository>();
        this.service = new SpecialEventService(this.repoMock.Object, this.ticketRepoMock.Object, attractionRepoMock.Object);
    }

    [TestMethod]
    public void Create_ValidData_ReturnsEvent()
    {
        var expected = new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);

        this.repoMock.Setup(r => r.Add(It.IsAny<Domain.SpecialEvent>())).Returns(expected);

        var result = this.service.Create("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);

        Assert.AreEqual(expected.Name, result.Name);
        Assert.AreEqual(expected.MaxCapacity, result.MaxCapacity);
        this.repoMock.Verify(r => r.Add(It.IsAny<Domain.SpecialEvent>()), Times.Once);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void GetAll_ReturnsListOfEvents()
    {
        var events = new List<Domain.SpecialEvent>
        {
            new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50),
            new Domain.SpecialEvent("Festival de Magia", DateTime.UtcNow.AddDays(5), 200, 100),
        };

        this.repoMock.Setup(r => r.GetAll()).Returns(events);

        var result = this.service.GetAll();

        Assert.AreEqual(2, result.Count());
        this.repoMock.Verify(r => r.GetAll(), Times.Once);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void GetById_EventExists_ReturnsEvent()
    {
        var eventId = Guid.NewGuid();
        var expected = new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);

        this.repoMock.Setup(r => r.GetById(eventId)).Returns(expected);

        var result = this.service.GetById(eventId);

        Assert.IsNotNull(result);
        Assert.AreEqual(expected.Name, result?.Name);
        this.repoMock.Verify(r => r.GetById(eventId), Times.Once);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void Delete_EventExists_DeletesEvent()
    {
        var eventId = Guid.NewGuid();
        var existingEvent = new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);

        this.repoMock.Setup(r => r.GetById(eventId)).Returns(existingEvent);
        this.repoMock.Setup(r => r.Delete(eventId));

        this.service.Delete(eventId);

        this.repoMock.Verify(r => r.Delete(eventId), Times.Once);
    }

    [TestMethod]
    public void AddAttractionToEvent_ValidData_AddsAttraction()
    {
        var eventId = Guid.NewGuid();
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var attractionId = attraction.Id;
        var specialEvent = new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);

        var attractionRepoMock = new Mock<IAttractionRepository>();
        attractionRepoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);
        this.repoMock.Setup(r => r.GetById(eventId)).Returns(specialEvent);
        this.repoMock.Setup(r => r.Update(eventId, It.IsAny<Domain.SpecialEvent>()));

        var service = new SpecialEventService(this.repoMock.Object, this.ticketRepoMock.Object, attractionRepoMock.Object);
        service.AddAttraction(eventId, attractionId);
        var result = service.GetById(eventId);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Attractions.Count);
        Assert.AreEqual(attraction, result.Attractions.First());
        this.repoMock.Verify(r => r.Update(eventId, specialEvent), Times.Once);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void AddAttraction_Should_Throw_When_Event_Not_Found()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();

        var attractionRepoMock = new Mock<IAttractionRepository>();
        this.repoMock.Setup(r => r.GetById(eventId))
            .Returns((Domain.SpecialEvent)null);

        var service = new SpecialEventService(this.repoMock.Object, this.ticketRepoMock.Object, attractionRepoMock.Object);

        // Act
        service.AddAttraction(eventId, attractionId);

        // Assert → handled by ExpectedException
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RemoveAttraction_Should_Throw_When_Event_Not_Found()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();

        this.repoMock.Setup(r => r.GetById(eventId))
            .Returns((Domain.SpecialEvent)null);

        // Act
        this.service.RemoveAttraction(eventId, attractionId);

        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void RemoveAttractionFromEvent_ValidData_RemovesAttraction()
    {
        var eventId = Guid.NewGuid();
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var specialEvent = new Domain.SpecialEvent("Noche Dinosaurios", DateTime.UtcNow.AddDays(1), 100, 50);
        specialEvent.AddAttraction(attraction);
        this.repoMock.Setup(r => r.GetById(eventId)).Returns(specialEvent);
        this.repoMock.Setup(r => r.Update(eventId, It.IsAny<Domain.SpecialEvent>()));
        this.service.RemoveAttraction(eventId, attraction.Id);
        var result = this.service.GetById(eventId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Attractions.Count);
        this.repoMock.Verify(r => r.Update(eventId, specialEvent), Times.Once);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void HasCapacity_WhenCapacitySufficient_ReturnsTrue()
    {
        var specialEvent = new Domain.SpecialEvent("Noche Terror", DateTime.UtcNow.AddDays(2), 100, 30);
        this.repoMock.Setup(r => r.GetById(specialEvent.Id)).Returns(specialEvent);
        this.ticketRepoMock.Setup(r => r.CountTicketsByEventId(specialEvent.Id)).Returns(50); // 50 tickets sold, 50 available

        var result = this.service.HasCapacity(specialEvent.Id, 5);

        Assert.IsTrue(result);
        this.repoMock.VerifyAll();
        this.ticketRepoMock.Verify(r => r.CountTicketsByEventId(specialEvent.Id), Times.Once);
    }

    [TestMethod]
    public void HasCapacity_WhenCapacityInsufficient_ReturnsFalse()
    {
        var specialEvent = new Domain.SpecialEvent("Noche Terror", DateTime.UtcNow.AddDays(2), 100, 30);
        this.repoMock.Setup(r => r.GetById(specialEvent.Id)).Returns(specialEvent);
        this.ticketRepoMock.Setup(r => r.CountTicketsByEventId(specialEvent.Id)).Returns(98); // 98 tickets sold, only 2 available

        var result = this.service.HasCapacity(specialEvent.Id, 5); // Requesting 5 tickets but only 2 available

        Assert.IsFalse(result);
        this.repoMock.VerifyAll();
        this.ticketRepoMock.Verify(r => r.CountTicketsByEventId(specialEvent.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void HasCapacity_Should_Throw_When_Event_Not_Found()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        this.repoMock.Setup(r => r.GetById(eventId))
            .Returns((Domain.SpecialEvent)null);

        // Act
        this.service.HasCapacity(eventId, 5);
        this.repoMock.VerifyAll();
    }

    [TestMethod]
    public void GetAvailableCapacity_WithTicketsSold_ReturnsCorrectAvailableCount()
    {
        // Arrange
        var specialEvent = new Domain.SpecialEvent("Noche Terror", DateTime.UtcNow.AddDays(2), 100, 30);
        var ticketsSold = 35;

        this.repoMock.Setup(r => r.GetById(specialEvent.Id)).Returns(specialEvent);
        this.ticketRepoMock.Setup(r => r.CountTicketsByEventId(specialEvent.Id)).Returns(ticketsSold);

        // Act
        var result = this.service.GetAvailableCapacity(specialEvent.Id);

        // Assert
        Assert.AreEqual(65, result); // 100 - 35 = 65 available
        this.repoMock.Verify(r => r.GetById(specialEvent.Id), Times.Once);
        this.ticketRepoMock.Verify(r => r.CountTicketsByEventId(specialEvent.Id), Times.Once);
    }

    [TestMethod]
    public void GetAvailableCapacity_WithNoTicketsSold_ReturnsFullCapacity()
    {
        // Arrange
        var specialEvent = new Domain.SpecialEvent("Festival de Magia", DateTime.UtcNow.AddDays(5), 200, 50);

        this.repoMock.Setup(r => r.GetById(specialEvent.Id)).Returns(specialEvent);
        this.ticketRepoMock.Setup(r => r.CountTicketsByEventId(specialEvent.Id)).Returns(0);

        // Act
        var result = this.service.GetAvailableCapacity(specialEvent.Id);

        // Assert
        Assert.AreEqual(200, result); // Full capacity available
        this.repoMock.Verify(r => r.GetById(specialEvent.Id), Times.Once);
        this.ticketRepoMock.Verify(r => r.CountTicketsByEventId(specialEvent.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAvailableCapacity_When_Event_Not_Found_Throws_Exception()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        this.repoMock.Setup(r => r.GetById(eventId))
            .Returns((Domain.SpecialEvent)null);

        // Act
        this.service.GetAvailableCapacity(eventId);

        // Assert is handled by ExpectedException
    }
}
