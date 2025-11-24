// <copyright file="MaintenanceMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Maintenance;
using VirtualPark.DTOs.Maintenance.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class MaintenanceMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapPreventiveMaintenanceToDto()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 30, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = "Mantenimiento preventivo de seguridad";

        var maintenance = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);

        var incidenceId = Guid.NewGuid();
        maintenance.SetAssociatedIncidence(incidenceId);

        // Act
        var dto = maintenance.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(maintenance.Id, dto.Id);
        Assert.AreEqual(attractionId, dto.AttractionId);
        Assert.AreEqual(scheduledDate, dto.ScheduledDate);
        Assert.AreEqual(startTime, dto.StartTime);
        Assert.AreEqual(estimatedDuration, dto.EstimatedDuration);
        Assert.AreEqual(description, dto.Description);
        Assert.AreEqual(incidenceId, dto.AssociatedIncidenceId);
    }

    [TestMethod]
    public void ToDto_ShouldMapMaintenanceWithoutAssociatedIncidence()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var maintenance = new PreventiveMaintenance(
            attractionId,
            new DateTime(2025, 11, 15),
            new TimeSpan(14, 0, 0),
            new TimeSpan(1, 30, 0),
            "Mantenimiento básico");

        // Act
        var dto = maintenance.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsNull(dto.AssociatedIncidenceId);
    }

    [TestMethod]
    public void ToDtoList_ShouldMapMultipleMaintenances()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var maintenances = new List<PreventiveMaintenance>
        {
            new PreventiveMaintenance(
                attractionId,
                new DateTime(2025, 11, 15),
                new TimeSpan(10, 0, 0),
                new TimeSpan(2, 0, 0),
                "Mantenimiento 1"),
            new PreventiveMaintenance(
                attractionId,
                new DateTime(2025, 11, 20),
                new TimeSpan(14, 0, 0),
                new TimeSpan(1, 0, 0),
                "Mantenimiento 2"),
        };

        // Act
        var dtos = maintenances.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Mantenimiento 1", dtos[0].Description);
        Assert.AreEqual("Mantenimiento 2", dtos[1].Description);
    }
}
