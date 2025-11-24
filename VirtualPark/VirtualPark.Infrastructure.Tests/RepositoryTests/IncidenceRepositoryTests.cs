// <copyright file="IncidenceRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class IncidenceRepositoryTests
{
    private IncidenceRepository repository = null!;
    private ParkDbContext context = null!;
    private Guid attractionId;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        this.context = new ParkDbContext(options);
        this.repository = new IncidenceRepository(this.context);

        // Create and save an attraction for testing
        var attraction = new Attraction("Test Attraction", "Description", "Type", 10, 50);
        this.context.Attractions.Add(attraction);
        this.context.SaveChanges();
        this.attractionId = attraction.Id;
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddNewIncidence()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Motor Failure",
            description: "Main engine malfunction",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);

        // Act
        var createdIncidence = this.repository.Add(incidence);

        // Assert
        Assert.IsNotNull(createdIncidence);
        Assert.AreEqual("Motor Failure", createdIncidence.Title);
        Assert.AreEqual("Main engine malfunction", createdIncidence.Description);
        Assert.IsTrue(createdIncidence.Status);
        Assert.AreEqual(this.attractionId, createdIncidence.AttractionId);
    }

    [TestMethod]
    public void Add_ShouldPersistIncidenceToDatabase()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Electrical Issue",
            description: "Power fluctuation detected",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);

        // Act
        this.repository.Add(incidence);

        // Assert
        var found = this.context.Incidences.FirstOrDefault(i => i.Id == incidence.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Electrical Issue", found.Title);
        Assert.AreEqual("Power fluctuation detected", found.Description);
    }

    [TestMethod]
    public void GetById_ShouldReturnIncidence_WhenExists()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Safety Check Required",
            description: "Routine inspection needed",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);

        // Act
        var result = this.repository.GetById(incidence.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(incidence.Id, result.Id);
        Assert.AreEqual("Safety Check Required", result.Title);
        Assert.AreEqual("Routine inspection needed", result.Description);
    }

    [TestMethod]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = this.repository.GetById(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByAttractionId_ShouldReturnAllIncidencesForAttraction()
    {
        // Arrange
        var incidence1 = new Incidence(
            title: "Issue 1",
            description: "Description 1",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        var incidence2 = new Incidence(
            title: "Issue 2",
            description: "Description 2",
            status: false,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);

        this.repository.Add(incidence1);
        this.repository.Add(incidence2);

        // Act
        var results = this.repository.GetByAttractionId(this.attractionId).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(i => i.Id == incidence1.Id));
        Assert.IsTrue(results.Any(i => i.Id == incidence2.Id));
    }

    [TestMethod]
    public void GetByAttractionId_ShouldReturnEmpty_WhenNoIncidencesExist()
    {
        // Arrange
        var emptyAttractionId = Guid.NewGuid();

        // Act
        var results = this.repository.GetByAttractionId(emptyAttractionId).ToList();

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void GetByAttractionId_ShouldOnlyReturnIncidencesForSpecificAttraction()
    {
        // Arrange - Create second attraction
        var attraction2 = new Attraction("Second Attraction", "Desc", "Type", 5, 30);
        this.context.Attractions.Add(attraction2);
        this.context.SaveChanges();

        var incidence1 = new Incidence("Issue A", "Desc A", true, DateTime.UtcNow, this.attractionId);
        var incidence2 = new Incidence("Issue B", "Desc B", true, DateTime.UtcNow, attraction2.Id);

        this.repository.Add(incidence1);
        this.repository.Add(incidence2);

        // Act
        var results = this.repository.GetByAttractionId(this.attractionId).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(incidence1.Id, results[0].Id);
        Assert.AreEqual(this.attractionId, results[0].AttractionId);
    }

    [TestMethod]
    public void Update_ShouldModifyExistingIncidence()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Original Title",
            description: "Original Description",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);

        // Modify the incidence (simulate closing it)
        incidence.Close();

        // Act
        this.repository.Update(incidence);

        // Assert
        this.context.ChangeTracker.Clear();
        var updated = this.context.Incidences.Find(incidence.Id);
        Assert.IsNotNull(updated);
        Assert.IsFalse(updated.Status); // Should be closed now
    }

    [TestMethod]
    public void Update_ShouldPersistChanges()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Test Incidence",
            description: "Test Description",
            status: false,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);

        // Reopen the incidence
        incidence.Reopen();

        // Act
        this.repository.Update(incidence);

        // Assert
        this.context.ChangeTracker.Clear();
        var updated = this.context.Incidences.Find(incidence.Id);
        Assert.IsNotNull(updated);
        Assert.IsTrue(updated.Status); // Should be open now
    }

    [TestMethod]
    public void Delete_ShouldRemoveIncidence_WhenExists()
    {
        // Arrange
        var incidence = new Incidence(
            title: "To Be Deleted",
            description: "This will be removed",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);

        // Act
        this.repository.Delete(incidence.Id);

        // Assert
        var result = this.context.Incidences.Find(incidence.Id);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Delete_ShouldNotThrow_WhenIncidenceDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw
        this.repository.Delete(nonExistentId);
    }

    [TestMethod]
    public void Add_MultipleIncidences_ShouldAllBeSaved()
    {
        // Arrange
        var incidence1 = new Incidence("Issue 1", "Desc 1", true, DateTime.UtcNow, this.attractionId);
        var incidence2 = new Incidence("Issue 2", "Desc 2", true, DateTime.UtcNow, this.attractionId);
        var incidence3 = new Incidence("Issue 3", "Desc 3", false, DateTime.UtcNow, this.attractionId);

        // Act
        this.repository.Add(incidence1);
        this.repository.Add(incidence2);
        this.repository.Add(incidence3);

        // Assert
        var allIncidences = this.repository.GetByAttractionId(this.attractionId).ToList();
        Assert.AreEqual(3, allIncidences.Count);
    }

    [TestMethod]
    public async Task Add_ShouldGenerateUniqueIds()
    {
        // Arrange
        var incidence1 = new Incidence("Issue 1", "Desc 1", true, DateTime.UtcNow, this.attractionId);
        var incidence2 = new Incidence("Issue 2", "Desc 2", true, DateTime.UtcNow, this.attractionId);

        // Act
        this.repository.Add(incidence1);
        this.repository.Add(incidence2);

        // Assert
        Assert.AreNotEqual(incidence1.Id, incidence2.Id);
        var count = await this.context.Incidences.CountAsync();
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void Update_AfterClosingIncidence_ShouldPersistStatusChange()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Active Issue",
            description: "This is currently active",
            status: true,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);
        Assert.IsTrue(incidence.Status);

        // Act - Close the incidence
        incidence.Close();
        this.repository.Update(incidence);

        // Assert
        this.context.ChangeTracker.Clear();
        var updated = this.repository.GetById(incidence.Id);
        Assert.IsNotNull(updated);
        Assert.IsFalse(updated.Status);
    }

    [TestMethod]
    public void Update_AfterReopeningIncidence_ShouldPersistStatusChange()
    {
        // Arrange
        var incidence = new Incidence(
            title: "Closed Issue",
            description: "This is currently closed",
            status: false,
            date: DateTime.UtcNow,
            attractionId: this.attractionId);
        this.repository.Add(incidence);
        Assert.IsFalse(incidence.Status);

        // Act - Reopen the incidence
        incidence.Reopen();
        this.repository.Update(incidence);

        // Assert
        this.context.ChangeTracker.Clear();
        var updated = this.repository.GetById(incidence.Id);
        Assert.IsNotNull(updated);
        Assert.IsTrue(updated.Status);
    }
}
