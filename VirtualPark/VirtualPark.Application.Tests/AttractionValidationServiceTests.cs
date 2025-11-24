// <copyright file="AttractionValidationServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using Moq;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class AttractionValidationServiceTests
{
    private Mock<IAttractionRepository> mockRepo = null!;
    private Mock<IMaintenanceRepository> mockMaintenanceRepo = null!;
    private Mock<IClockService> mockClockService = null!;
    private AttractionValidationService service = null!;
    private Guid attractionId;
    private readonly DateTime fixedNow = new(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    [TestInitialize]
    public void Setup()
    {
        this.mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        this.mockMaintenanceRepo = new Mock<IMaintenanceRepository>(MockBehavior.Strict);
        this.mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        this.mockClockService.Setup(c => c.GetNow()).Returns(this.fixedNow);
        this.mockMaintenanceRepo
            .Setup(m => m.GetByAttractionId(It.IsAny<Guid>()))
            .Returns(Enumerable.Empty<PreventiveMaintenance>());
        this.service = new AttractionValidationService(
            this.mockRepo.Object,
            this.mockMaintenanceRepo.Object,
            this.mockClockService.Object);
        this.attractionId = Guid.NewGuid();
    }

    [TestMethod]
    public void HasValidAge_ReturnsTrue_WhenVisitorMeetsMinimumAge()
    {
        // Arrange
        var attraction = new Attraction("Roller Coaster", "Fast ride", "RollerCoaster", 12, 30) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.HasValidAge(this.attractionId, 15);

        // Assert
        Assert.IsTrue(result);
        this.mockRepo.VerifyAll();
    }

    [TestMethod]
    public void HasValidAge_ReturnsFalse_WhenVisitorBelowMinimumAge()
    {
        // Arrange
        var attraction = new Attraction("Roller Coaster", "Fast ride", "RollerCoaster", 12, 30) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.HasValidAge(this.attractionId, 10);

        // Assert
        Assert.IsFalse(result);
        this.mockRepo.VerifyAll();
    }

    [TestMethod]
    public void HasValidAge_ReturnsTrue_WhenVisitorExactlyMinimumAge()
    {
        // Arrange
        var attraction = new Attraction("Roller Coaster", "Fast ride", "RollerCoaster", 12, 30) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.HasValidAge(this.attractionId, 12);

        // Assert
        Assert.IsTrue(result);
        this.mockRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void HasValidAge_ThrowsException_WhenAttractionNotFound()
    {
        // Arrange
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns((Attraction)null!);

        // Act
        this.service.HasValidAge(this.attractionId, 15);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsTrue_WhenNoIncidences()
    {
        // Arrange
        var attraction = new Attraction("Ferris Wheel", "Nice view", "Wheel", 0, 40) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsTrue(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsFalse_WhenHasOpenIncidence()
    {
        // Arrange
        var attraction = new Attraction("Haunted House", "Scary", "House", 10, 20) { Id = this.attractionId };
        attraction.AddIncidence("Broken Door", "Door is stuck", true, DateTime.Now);
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsFalse(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsTrue_WhenHasOnlyClosedIncidences()
    {
        // Arrange
        var attraction = new Attraction("Water Slide", "Fun", "WaterRide", 8, 15) { Id = this.attractionId };
        attraction.AddIncidence("Fixed Issue", "Was broken", false, DateTime.Now);
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsTrue(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void IsAttractionAvailable_ThrowsException_WhenAttractionNotFound()
    {
        // Arrange
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns((Attraction)null!);

        // Act
        this.service.IsAttractionAvailable(this.attractionId);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsFalse_WhenMultipleIncidencesWithAtLeastOneOpen()
    {
        // Arrange
        var attraction = new Attraction("Bumper Cars", "Fun collision", "Cars", 5, 20) { Id = this.attractionId };
        attraction.AddIncidence("Old Issue", "Fixed", false, DateTime.Now.AddDays(-1));
        attraction.AddIncidence("Current Issue", "Needs fixing", true, DateTime.Now);
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsFalse(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsFalse_WhenUnderMaintenanceWindow()
    {
        // Arrange
        var attraction = new Attraction("Swing Ride", "Gentle", "Swing", 0, 20) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);
        var maintenance = new PreventiveMaintenance(
            this.attractionId,
            this.fixedNow.Date,
            this.fixedNow.TimeOfDay,
            TimeSpan.FromHours(1),
            "Scheduled");
        this.mockMaintenanceRepo
            .Setup(m => m.GetByAttractionId(this.attractionId))
            .Returns(new[] { maintenance });

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsFalse(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsTrue_WhenMaintenanceIncidenceNotInWindow()
    {
        // Arrange
        var attraction = new Attraction("Carousel", "Spin", "Carousel", 0, 30) { Id = this.attractionId };
        this.mockRepo.Setup(r => r.GetById(this.attractionId)).Returns(attraction);
        var incidence = attraction.AddIncidence("Scheduled Work", "Testing", true, DateTime.Now);
        var maintenance = new PreventiveMaintenance(
            this.attractionId,
            this.fixedNow.Date.AddDays(-2),
            this.fixedNow.TimeOfDay,
            TimeSpan.FromHours(1),
            "Past maintenance");
        maintenance.SetAssociatedIncidence(incidence.Id);
        this.mockMaintenanceRepo
            .Setup(m => m.GetByAttractionId(this.attractionId))
            .Returns(new[] { maintenance });

        // Act
        var result = this.service.IsAttractionAvailable(this.attractionId);

        // Assert
        Assert.IsTrue(result);
        this.mockRepo.VerifyAll();
        this.mockMaintenanceRepo.Verify(m => m.GetByAttractionId(this.attractionId), Times.Once);
    }
}
