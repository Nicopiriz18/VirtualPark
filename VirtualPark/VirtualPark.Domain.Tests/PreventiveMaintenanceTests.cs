// <copyright file="PreventiveMaintenanceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Tests;

[TestClass]
public class PreventiveMaintenanceTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 30, 0);
        var description = "Mantenimiento preventivo de seguridad";

        // Act
        var maintenance = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);

        // Assert
        Assert.IsNotNull(maintenance);
        Assert.AreNotEqual(Guid.Empty, maintenance.Id);
        Assert.AreEqual(attractionId, maintenance.AttractionId);
        Assert.AreEqual(scheduledDate, maintenance.ScheduledDate);
        Assert.AreEqual(startTime, maintenance.StartTime);
        Assert.AreEqual(estimatedDuration, maintenance.EstimatedDuration);
        Assert.AreEqual(description, maintenance.Description);
        Assert.IsNull(maintenance.AssociatedIncidenceId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNegativeDuration_ThrowsArgumentException()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(-1, 0, 0);
        var description = "Mantenimiento preventivo";

        // Act
        _ = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithZeroDuration_ThrowsArgumentException()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = TimeSpan.Zero;
        var description = "Mantenimiento preventivo";

        // Act
        _ = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullDescription_ThrowsArgumentNullException()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        string description = null!;

        // Act
        _ = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithEmptyDescription_ThrowsArgumentException()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = string.Empty;

        // Act
        _ = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithEmptyAttractionId_ThrowsArgumentException()
    {
        // Arrange
        var attractionId = Guid.Empty;
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = "Mantenimiento preventivo";

        // Act
        _ = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);
    }

    [TestMethod]
    public void SetAssociatedIncidence_WithValidId_SetsProperty()
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
        var incidenceId = Guid.NewGuid();

        // Act
        maintenance.SetAssociatedIncidence(incidenceId);

        // Assert
        Assert.AreEqual(incidenceId, maintenance.AssociatedIncidenceId);
    }
}
