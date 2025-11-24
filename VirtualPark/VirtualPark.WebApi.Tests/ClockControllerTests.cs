// <copyright file="ClockControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Clock;
using VirtualPark.DTOs.Common;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;
[TestClass]
public class ClockControllerTests
{
    [TestMethod]
    public void Get_ReturnsCurrentClock()
    {
        var expectedNow = DateTime.Parse("2025-09-02T14:45");
        var mockService = new Mock<IClockService>();
        mockService.Setup(s => s.GetNow()).Returns(expectedNow);

        var controller = new ClockController(mockService.Object);

        var result = controller.Get() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var clockResponse = result.Value as ClockResponseDto;
        Assert.IsNotNull(clockResponse);
        Assert.AreEqual(expectedNow, clockResponse.CurrentDateTime);
    }

    [TestMethod]
    public void Put_ValidValue_ReturnsNoContent()
    {
        var mockService = new Mock<IClockService>();
        mockService.Setup(s => s.SetNow(It.IsAny<DateTime>()));

        var controller = new ClockController(mockService.Object);
        var body = new ClockRequestDto { Value = "2025-09-02T14:45" };

        var result = controller.Put(body);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.Verify(s => s.SetNow(It.IsAny<DateTime>()), Times.Once);
    }

    [TestMethod]
    public void Put_InvalidValue_ReturnsBadRequest()
    {
        var mockService = new Mock<IClockService>(MockBehavior.Strict);
        var controller = new ClockController(mockService.Object);
        var body = new ClockRequestDto { Value = "invalid-date" };
        var result = controller.Put(body) as BadRequestObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.AreEqual("Invalid format. Expected YYYY-MM-DDTHH:MM", result.Value);
        mockService.Verify(s => s.SetNow(It.IsAny<DateTime>()), Times.Never);
    }
}
