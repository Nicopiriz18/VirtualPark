// <copyright file="AttractionRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class AttractionRepositoryTests
{
    private ParkDbContext context = null!;
    private AttractionRepository repository = null!;
    private IncidenceRepository incidenceRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new AttractionRepository(this.context);
        this.incidenceRepository = new IncidenceRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public async Task AddAsync_Should_Add_Attraction_To_Database()
    {
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);

        this.repository.Add(attraction);

        var saved = await this.context.Attractions.FindAsync(attraction.Id);
        Assert.IsNotNull(saved);
        Assert.AreEqual("RollerCoaster", saved.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_Should_Return_Attraction_When_Exists()
    {
        var attraction = new Attraction("Simulador 3D", "VR Experience", "Simulador", 10, 20);
        this.context.Attractions.Add(attraction);
        await this.context.SaveChangesAsync();

        var result = this.repository.GetById(attraction.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Simulador 3D", result.Name);
    }

    [TestMethod]
    public async Task DeleteAsync_Should_Remove_Attraction_When_Exists()
    {
        var attraction = new Attraction("Casa del Terror", "Scary fun", "Espectáculo", 15, 30);
        this.context.Attractions.Add(attraction);
        await this.context.SaveChangesAsync();

        this.repository.Delete(attraction.Id);

        var result = await this.context.Attractions.FindAsync(attraction.Id);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_Should_Return_All_Attractions()
    {
        var attraction1 = new Attraction("Attraction1", "Desc1", "Type1", 5, 10);
        var attraction2 = new Attraction("Attraction2", "Desc2", "Type2", 10, 20);
        this.context.Attractions.AddRange(attraction1, attraction2);
        this.context.SaveChanges();
        var results = this.repository.GetAll().ToList();
        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(a => a.Name == "Attraction1"));
        Assert.IsTrue(results.Any(a => a.Name == "Attraction2"));
    }

    [TestMethod]
    public async Task Update_Should_Modify_Existing_Attraction()
    {
        var attraction = new Attraction("OldName", "OldDesc", "OldType", 5, 10);
        this.context.Attractions.Add(attraction);
        await this.context.SaveChangesAsync();
        attraction.ModifyAttraction("NewName", "NewDesc", "NewType", 10, 20);
        this.repository.Update(attraction.Id, attraction);
        var updated = await this.context.Attractions.FindAsync(attraction.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual("NewName", updated.Name);
    }

    [TestMethod]
    public async Task AddIncidence_Should_Save_Incidence_To_Attraction()
    {
        // Arrange - Create and save attraction
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Extreme", 12, 30);
        this.context.Attractions.Add(attraction);
        await this.context.SaveChangesAsync();

        // Act - Create and save incidence using IncidenceRepository
        var incidenceDate = DateTime.UtcNow;
        var incidence = new Incidence(
            "Motor Failure",
            "Main engine malfunction",
            true,
            incidenceDate,
            attraction.Id);

        this.incidenceRepository.Add(incidence);

        // Assert - Verify incidence was saved
        this.context.ChangeTracker.Clear(); // Clear tracking to force fresh load
        var savedIncidence = await this.context.Incidences
            .FirstOrDefaultAsync(i => i.Id == incidence.Id);

        Assert.IsNotNull(savedIncidence);
        Assert.AreEqual("Motor Failure", savedIncidence.Title);
        Assert.AreEqual("Main engine malfunction", savedIncidence.Description);
        Assert.IsTrue(savedIncidence.Status);
        Assert.AreEqual(attraction.Id, savedIncidence.AttractionId);

        // Verify incidence is associated with the attraction
        var updatedAttraction = await this.context.Attractions
            .Include(a => a.Incidences)
            .FirstOrDefaultAsync(a => a.Id == attraction.Id);

        Assert.IsNotNull(updatedAttraction);
        Assert.AreEqual(1, updatedAttraction.Incidences.Count);
        Assert.AreEqual(incidence.Id, updatedAttraction.Incidences.First().Id);
    }
}
