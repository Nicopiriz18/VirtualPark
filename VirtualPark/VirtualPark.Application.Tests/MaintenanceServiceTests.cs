// <copyright file="MaintenanceServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Application.Maintenance;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class MaintenanceServiceTests
{
    private Mock<IMaintenanceRepository> mockMaintenanceRepo = null!;
    private Mock<IAttractionRepository> mockAttractionRepo = null!;
    private Mock<IAttractionIncidenceService> mockIncidenceService = null!;
    private Mock<IClockService> mockClockService = null!;
    private MaintenanceService service = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockMaintenanceRepo = new Mock<IMaintenanceRepository>(MockBehavior.Strict);
        this.mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        this.mockIncidenceService = new Mock<IAttractionIncidenceService>(MockBehavior.Strict);
        this.mockClockService = new Mock<IClockService>(MockBehavior.Strict);

        this.service = new MaintenanceService(
            this.mockMaintenanceRepo.Object,
            this.mockAttractionRepo.Object,
            this.mockIncidenceService.Object,
            this.mockClockService.Object);
    }

    [TestMethod]
    public void ScheduleMaintenance_CreatesMaintenanceAndIncidence_WhenAttractionExists()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = "Mantenimiento preventivo de seguridad";

        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50)
        {
            Id = attractionId,
        };

        PreventiveMaintenance? capturedMaintenance = null;
        var mockIncidence = new Incidence("Mantenimiento Programado", description, true, scheduledDate, attractionId);

        this.mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns(attraction);
        this.mockIncidenceService.Setup(s => s.AddIncidence(
            attractionId,
            "Mantenimiento Programado",
            description,
            true,
            scheduledDate)).Returns(mockIncidence);
        this.mockMaintenanceRepo.Setup(r => r.Add(It.IsAny<PreventiveMaintenance>()))
            .Callback<PreventiveMaintenance>(m => capturedMaintenance = m)
            .Returns((PreventiveMaintenance m) => m);
        this.mockMaintenanceRepo.Setup(r => r.Update(It.IsAny<PreventiveMaintenance>()));

        // Act
        var result = this.service.ScheduleMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(attractionId, result.AttractionId);
        Assert.AreEqual(scheduledDate, result.ScheduledDate);
        Assert.AreEqual(startTime, result.StartTime);
        Assert.AreEqual(estimatedDuration, result.EstimatedDuration);
        Assert.AreEqual(description, result.Description);
        Assert.AreEqual(mockIncidence.Id, result.AssociatedIncidenceId);

        this.mockAttractionRepo.VerifyAll();
        this.mockIncidenceService.VerifyAll();
        this.mockMaintenanceRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ScheduleMaintenance_ThrowsException_WhenAttractionDoesNotExist()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var scheduledDate = new DateTime(2025, 11, 15);
        var startTime = new TimeSpan(10, 0, 0);
        var estimatedDuration = new TimeSpan(2, 0, 0);
        var description = "Mantenimiento preventivo";

        this.mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns((Attraction?)null);

        // Act
        this.service.ScheduleMaintenance(attractionId, scheduledDate, startTime, estimatedDuration, description);
    }

    [TestMethod]
    public void GetMaintenanceById_ReturnsMaintenance_WhenExists()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var maintenance = new PreventiveMaintenance(
            attractionId,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");

        this.mockMaintenanceRepo.Setup(r => r.GetById(maintenance.Id)).Returns(maintenance);

        // Act
        var result = this.service.GetMaintenanceById(maintenance.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(maintenance.Id, result.Id);
        this.mockMaintenanceRepo.VerifyAll();
    }

    [TestMethod]
    public void GetMaintenanceById_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();

        this.mockMaintenanceRepo.Setup(r => r.GetById(maintenanceId)).Returns((PreventiveMaintenance?)null);

        // Act
        var result = this.service.GetMaintenanceById(maintenanceId);

        // Assert
        Assert.IsNull(result);
        this.mockMaintenanceRepo.VerifyAll();
    }

    [TestMethod]
    public void GetMaintenancesByAttraction_ReturnsMaintenances()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var maintenances = new List<PreventiveMaintenance>
        {
            new PreventiveMaintenance(attractionId, new DateTime(2025, 11, 15), new TimeSpan(10, 0, 0), new TimeSpan(2, 0, 0), "Mantenimiento 1"),
            new PreventiveMaintenance(attractionId, new DateTime(2025, 11, 20), new TimeSpan(14, 0, 0), new TimeSpan(1, 0, 0), "Mantenimiento 2"),
        };

        this.mockMaintenanceRepo.Setup(r => r.GetByAttractionId(attractionId)).Returns(maintenances);

        // Act
        var result = this.service.GetMaintenancesByAttraction(attractionId);

        // Assert
        Assert.AreEqual(2, result.Count());
        this.mockMaintenanceRepo.VerifyAll();
    }

    [TestMethod]
    public void CancelMaintenance_RemovesMaintenanceAndClosesIncidence()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidenceId = Guid.NewGuid();

        var maintenance = new PreventiveMaintenance(
            attractionId,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");
        maintenance.SetAssociatedIncidence(incidenceId);

        this.mockMaintenanceRepo.Setup(r => r.GetById(maintenance.Id)).Returns(maintenance);
        this.mockIncidenceService.Setup(s => s.CloseIncidence(attractionId, incidenceId));
        this.mockMaintenanceRepo.Setup(r => r.Delete(maintenance.Id));

        // Act
        this.service.CancelMaintenance(maintenance.Id);

        // Assert
        this.mockMaintenanceRepo.VerifyAll();
        this.mockIncidenceService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void CancelMaintenance_ThrowsException_WhenMaintenanceDoesNotExist()
    {
        // Arrange
        var maintenanceId = Guid.NewGuid();

        this.mockMaintenanceRepo.Setup(r => r.GetById(maintenanceId)).Returns((PreventiveMaintenance?)null);

        // Act
        this.service.CancelMaintenance(maintenanceId);
    }
}
