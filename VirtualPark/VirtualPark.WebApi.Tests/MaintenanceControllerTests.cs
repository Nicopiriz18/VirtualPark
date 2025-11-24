// <copyright file="MaintenanceControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Maintenance;
using VirtualPark.Domain;
using VirtualPark.DTOs.Maintenance.Requests;
using VirtualPark.DTOs.Maintenance.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class MaintenanceControllerTests
{
    [TestMethod]
    public void Post_ScheduleMaintenance_ReturnsCreatedAtAction()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = "Mantenimiento preventivo";

        var maintenance = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);

        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.ScheduleMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description)).Returns(maintenance);

        var controller = new MaintenanceController(mockService.Object);

        var request = new CreateMaintenanceRequestDto
        {
            AttractionId = attractionId,
            ScheduledDate = scheduledDate,
            StartTime = startTime,
            EstimatedDuration = estimatedDuration,
            Description = description,
        };

        // Act
        var result = controller.Post(request) as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        var dto = result.Value as MaintenanceDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(maintenance.Id, dto.Id);
        Assert.AreEqual(attractionId, dto.AttractionId);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetById_ReturnsMaintenance_WhenExists()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var maintenance = new PreventiveMaintenance(
            attractionId,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");

        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetMaintenanceById(maintenance.Id)).Returns(maintenance);

        var controller = new MaintenanceController(mockService.Object);

        // Act
        var result = controller.Get(maintenance.Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var dto = result.Value as MaintenanceDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(maintenance.Id, dto.Id);
        Assert.AreEqual(attractionId, dto.AttractionId);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetById_ReturnsNotFound_WhenMaintenanceDoesNotExist()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();
        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetMaintenanceById(maintenanceId)).Returns((PreventiveMaintenance?)null);

        var controller = new MaintenanceController(mockService.Object);

        // Act
        var result = controller.Get(maintenanceId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetByAttractionId_ReturnsMaintenances()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var maintenances = new List<PreventiveMaintenance>
        {
            new PreventiveMaintenance(attractionId, new DateTime(2025, 11, 15), new TimeSpan(10, 0, 0), new TimeSpan(2, 0, 0), "Mantenimiento 1"),
            new PreventiveMaintenance(attractionId, new DateTime(2025, 11, 20), new TimeSpan(14, 0, 0), new TimeSpan(1, 0, 0), "Mantenimiento 2"),
        };

        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetMaintenancesByAttraction(attractionId)).Returns(maintenances);

        var controller = new MaintenanceController(mockService.Object);

        // Act
        var result = controller.GetByAttraction(attractionId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var dtos = result.Value as List<MaintenanceDto>;
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void Delete_CancelsMaintenance_ReturnsNoContent()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();
        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.CancelMaintenance(maintenanceId));

        var controller = new MaintenanceController(mockService.Object);

        // Act
        var result = controller.Delete(maintenanceId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        mockService.VerifyAll();
    }

    [TestMethod]
    public void Delete_ReturnsNotFound_WhenMaintenanceDoesNotExist()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();
        var mockService = new Mock<IMaintenanceService>(MockBehavior.Strict);
        mockService.Setup(s => s.CancelMaintenance(maintenanceId)).Throws(new KeyNotFoundException());

        var controller = new MaintenanceController(mockService.Object);

        // Act
        var result = controller.Delete(maintenanceId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        mockService.VerifyAll();
    }
}
