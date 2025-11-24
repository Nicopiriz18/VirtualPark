// <copyright file="SpecialEventControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.SpecialEvent;
using VirtualPark.Domain;
using VirtualPark.DTOs.SpecialEvents.Requests;
using VirtualPark.DTOs.SpecialEvents.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class SpecialEventControllerTests
{
    [TestMethod]
    public void GetAllSpecialEvents_ReturnsOkResult()
    {
        // Arrange
        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.GetAll()).Returns(
        [
            new SpecialEvent("Event1", DateTime.UtcNow, 12, 13),
            new SpecialEvent("Event2", DateTime.UtcNow, 12, 13)
        ]);

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.Get();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var events = okResult.Value as IEnumerable<SpecialEventDto>;
        Assert.IsNotNull(events);
        Assert.AreEqual(2, events.Count());
    }

    [TestMethod]
    public void GetSpecialEventById_ReturnsOkResult_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.GetById(eventId))
            .Returns(new SpecialEvent("Event1", DateTime.UtcNow, 12, 13));

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.Get(eventId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var specialEventDto = okResult.Value as SpecialEventDetailDto;
        Assert.IsNotNull(specialEventDto);
        Assert.AreEqual("Event1", specialEventDto.Name);
    }

    [TestMethod]
    public void GetSpecialEvent_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.GetById(eventId)).Returns((SpecialEvent)null);

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.Get(eventId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void PostSpecialEvent_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var newEvent = new SpecialEvent("Event1", DateTime.UtcNow, 12, 13);
        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.Create(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<decimal>()))
            .Returns(newEvent);

        var controller = new SpecialEventController(mockService.Object);

        var request = new VirtualPark.DTOs.SpecialEvents.Requests.CreateSpecialEventRequestDto
        {
            Name = "Event1",
            Date = DateTime.UtcNow,
            MaxCapacity = 12,
            AdditionalCost = 13,
        };

        // Act
        var result = controller.Post(request);

        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        var specialEventDto = createdAtActionResult.Value as SpecialEventDto;
        Assert.IsNotNull(specialEventDto);
        Assert.AreEqual("Event1", specialEventDto.Name);
    }

    [TestMethod]
    public void DeleteSpecialEvent_ReturnsNoContentResult_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.Delete(eventId));

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.Delete(eventId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.Verify(service => service.Delete(eventId), Times.Once);
    }

    [TestMethod]
    public void AddAttractionToEvent_ReturnsNoContentResult_WhenSuccessful()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var specialEvent = new SpecialEvent("Event1", DateTime.UtcNow, 100, 50);

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.AddAttraction(eventId, It.IsAny<Guid>()));
        mockService.Setup(service => service.GetById(eventId)).Returns(specialEvent);

        var controller = new SpecialEventController(mockService.Object);

        var request = new AddAttractionToEventRequestDto
        {
            AttractionId = attractionId,
        };

        // Act
        var result = controller.AddAttractionToEvent(eventId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        mockService.Verify(service => service.AddAttraction(eventId, It.IsAny<Guid>()), Times.Once);
    }

    [TestMethod]
    public void AddAttractionToEvent_ReturnsNotFound_WhenKeyNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.AddAttraction(eventId, It.IsAny<Guid>()))
            .Throws(new KeyNotFoundException());

        var controller = new SpecialEventController(mockService.Object);

        var request = new AddAttractionToEventRequestDto
        {
            AttractionId = attractionId,
        };

        // Act
        var result = controller.AddAttractionToEvent(eventId, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void AddAttractionToEvent_ReturnsBadRequest_WhenInvalidOperationException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var errorMessage = "Cannot add attraction to this event";

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.AddAttraction(eventId, It.IsAny<Guid>()))
            .Throws(new InvalidOperationException(errorMessage));

        var controller = new SpecialEventController(mockService.Object);

        var request = new AddAttractionToEventRequestDto
        {
            AttractionId = attractionId,
        };

        // Act
        var result = controller.AddAttractionToEvent(eventId, request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(errorMessage, badRequestResult.Value);
    }

    [TestMethod]
    public void RemoveAttractionFromEvent_ReturnsOk_WhenAttractionIsRemovedSuccessfully()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var updatedEvent = new SpecialEvent("Test Event", DateTime.Now, 100, 50);

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.RemoveAttraction(eventId, attractionId));
        mockService.Setup(service => service.GetById(eventId)).Returns(updatedEvent);

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.RemoveAttractionFromEvent(eventId, attractionId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var eventDto = okResult.Value as SpecialEventDetailDto;
        Assert.IsNotNull(eventDto);
        Assert.AreEqual("Test Event", eventDto.Name);
    }

    [TestMethod]
    public void RemoveAttractionFromEvent_ReturnsNotFound_WhenKeyNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.RemoveAttraction(eventId, attractionId))
            .Throws(new KeyNotFoundException());

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.RemoveAttractionFromEvent(eventId, attractionId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void RemoveAttractionFromEvent_ReturnsBadRequest_WhenInvalidOperationException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var errorMessage = "Cannot remove attraction from this event";

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.RemoveAttraction(eventId, attractionId))
            .Throws(new InvalidOperationException(errorMessage));

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.RemoveAttractionFromEvent(eventId, attractionId);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(errorMessage, badRequestResult.Value);
    }

    [TestMethod]
    public void CheckCapacity_ReturnsOk_WhenEventHasCapacity()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var ticketsRequested = 5;
        var specialEvent = new SpecialEvent("Test Event", DateTime.Now, 100, 50);

        // Usar reflexión para asignar el Id
        typeof(SpecialEvent).GetProperty("Id")!.SetValue(specialEvent, eventId);

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.GetById(eventId)).Returns(specialEvent);
        mockService.Setup(service => service.GetAvailableCapacity(eventId)).Returns(50);
        mockService.Setup(service => service.HasCapacity(eventId, ticketsRequested))
            .Returns(true);

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.CheckCapacity(eventId, ticketsRequested);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var capacityDto = okResult.Value as SpecialEventCapacityDto;
        Assert.IsNotNull(capacityDto);
        Assert.AreEqual(eventId, capacityDto.EventId);
        Assert.IsTrue(capacityDto.HasCapacity);
    }

    [TestMethod]
    public void CheckCapacity_ReturnsOk_WhenEventHasNoCapacity()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var ticketsRequested = 10;
        var specialEvent = new SpecialEvent("Test Event", DateTime.Now, 100, 50);

        // Usar reflexión para asignar el Id
        typeof(SpecialEvent).GetProperty("Id")!.SetValue(specialEvent, eventId);

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.GetById(eventId)).Returns(specialEvent);
        mockService.Setup(service => service.GetAvailableCapacity(eventId)).Returns(5);
        mockService.Setup(service => service.HasCapacity(eventId, ticketsRequested))
            .Returns(false);

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.CheckCapacity(eventId, ticketsRequested);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var capacityDto = okResult.Value as SpecialEventCapacityDto;
        Assert.IsNotNull(capacityDto);
        Assert.AreEqual(eventId, capacityDto.EventId);
        Assert.IsFalse(capacityDto.HasCapacity);
    }

    [TestMethod]
    public void CheckCapacity_ReturnsNotFound_WhenKeyNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var ticketsRequested = 5;

        var mockService = new Mock<ISpecialEventService>();
        mockService.Setup(service => service.HasCapacity(eventId, ticketsRequested))
            .Throws(new KeyNotFoundException());

        var controller = new SpecialEventController(mockService.Object);

        // Act
        var result = controller.CheckCapacity(eventId, ticketsRequested);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}
