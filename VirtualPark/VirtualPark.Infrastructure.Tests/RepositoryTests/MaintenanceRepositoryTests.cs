// <copyright file="MaintenanceRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class MaintenanceRepositoryTests
{
    private ParkDbContext context = null!;
    private MaintenanceRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new MaintenanceRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public async Task Add_Should_Add_Maintenance_To_Database()
    {
        // Arrange
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        await this.context.Attractions.AddAsync(attraction);
        await this.context.SaveChangesAsync();

        var maintenance = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo de seguridad");

        // Act
        this.repository.Add(maintenance);

        // Assert
        var saved = await this.context.PreventiveMaintenances.FindAsync(maintenance.Id);
        Assert.IsNotNull(saved);
        Assert.AreEqual(attraction.Id, saved.AttractionId);
        Assert.AreEqual("Mantenimiento preventivo de seguridad", saved.Description);
    }

    [TestMethod]
    public async Task GetById_Should_Return_Maintenance_When_Exists()
    {
        // Arrange
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        await this.context.Attractions.AddAsync(attraction);
        await this.context.SaveChangesAsync();

        var maintenance = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");

        await this.context.PreventiveMaintenances.AddAsync(maintenance);
        await this.context.SaveChangesAsync();

        // Act
        var result = this.repository.GetById(maintenance.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(maintenance.Id, result.Id);
        Assert.AreEqual("Mantenimiento preventivo", result.Description);
    }

    [TestMethod]
    public void GetById_Should_Return_Null_When_Not_Exists()
    {
        // Act
        var result = this.repository.GetById(Guid.NewGuid());

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByAttractionId_Should_Return_All_Maintenances_For_Attraction()
    {
        // Arrange
        var attraction1 = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        var attraction2 = new Attraction("Simulador", "VR ride", "Simulador", 10, 30);
        await this.context.Attractions.AddRangeAsync(attraction1, attraction2);
        await this.context.SaveChangesAsync();

        var maintenance1 = new PreventiveMaintenance(
            attraction1.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento 1");

        var maintenance2 = new PreventiveMaintenance(
            attraction1.Id,
            new DateTime(2025, 11, 20),
            new TimeSpan(14, 0, 0),
            new TimeSpan(1, 0, 0),
            "Mantenimiento 2");

        var maintenance3 = new PreventiveMaintenance(
            attraction2.Id,
            new DateTime(2025, 11, 25),
            new TimeSpan(9, 0, 0),
            new TimeSpan(3, 0, 0),
            "Mantenimiento 3");

        await this.context.PreventiveMaintenances.AddRangeAsync(maintenance1, maintenance2, maintenance3);
        await this.context.SaveChangesAsync();

        // Act
        var result = this.repository.GetByAttractionId(attraction1.Id);

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(m => m.AttractionId == attraction1.Id));
    }

    [TestMethod]
    public async Task GetAll_Should_Return_All_Maintenances()
    {
        // Arrange
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        await this.context.Attractions.AddAsync(attraction);
        await this.context.SaveChangesAsync();

        var maintenance1 = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento 1");

        var maintenance2 = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 20),
            new TimeSpan(14, 0, 0),
            new TimeSpan(1, 0, 0),
            "Mantenimiento 2");

        await this.context.PreventiveMaintenances.AddRangeAsync(maintenance1, maintenance2);
        await this.context.SaveChangesAsync();

        // Act
        var result = this.repository.GetAll();

        // Assert
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task Update_Should_Update_Maintenance()
    {
        // Arrange
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        await this.context.Attractions.AddAsync(attraction);
        await this.context.SaveChangesAsync();

        var maintenance = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");

        await this.context.PreventiveMaintenances.AddAsync(maintenance);
        await this.context.SaveChangesAsync();

        var incidenceId = Guid.NewGuid();

        // Act
        maintenance.SetAssociatedIncidence(incidenceId);
        this.repository.Update(maintenance);

        // Assert
        var updated = await this.context.PreventiveMaintenances.FindAsync(maintenance.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual(incidenceId, updated.AssociatedIncidenceId);
    }

    [TestMethod]
    public async Task Delete_Should_Remove_Maintenance()
    {
        // Arrange
        var attraction = new Attraction("RollerCoaster", "Fast ride", "Montaña Rusa", 12, 50);
        await this.context.Attractions.AddAsync(attraction);
        await this.context.SaveChangesAsync();

        var maintenance = new PreventiveMaintenance(
            attraction.Id,
            new DateTime(2025, 11, 15),
            new TimeSpan(10, 0, 0),
            new TimeSpan(2, 0, 0),
            "Mantenimiento preventivo");

        await this.context.PreventiveMaintenances.AddAsync(maintenance);
        await this.context.SaveChangesAsync();

        // Act
        this.repository.Delete(maintenance.Id);

        // Assert
        var deleted = await this.context.PreventiveMaintenances.FindAsync(maintenance.Id);
        Assert.IsNull(deleted);
    }
}
