// <copyright file="ClockRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class ClockRepositoryTests
{
    private ParkDbContext context = null!;
    private ClockRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new ClockRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void GetConfiguration_ReturnsNull_WhenNoConfigurationExists()
    {
        // Act
        var result = this.repository.GetConfiguration();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void SaveConfiguration_CreatesNewConfiguration_WhenNoneExists()
    {
        // Arrange
        var config = new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-09-10T10:00"),
        };

        // Act
        this.repository.SaveConfiguration(config);

        // Assert
        var saved = this.context.ClockConfigurations.FirstOrDefault();
        Assert.IsNotNull(saved);
        Assert.AreEqual(config.CustomDateTime, saved.CustomDateTime);
    }

    [TestMethod]
    public void SaveConfiguration_UpdatesExistingConfiguration()
    {
        // Arrange: Crear una configuración inicial
        var initialConfig = new ClockConfiguration
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            CustomDateTime = DateTime.Parse("2025-09-10T10:00"),
        };
        this.context.ClockConfigurations.Add(initialConfig);
        this.context.SaveChanges();

        // Act: Actualizar con nuevo valor
        var updatedConfig = new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-12-25T23:59:59"),
        };
        this.repository.SaveConfiguration(updatedConfig);

        // Assert
        var saved = this.context.ClockConfigurations
            .FirstOrDefault(c => c.Id == Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.IsNotNull(saved);
        Assert.AreEqual(DateTime.Parse("2025-12-25T23:59:59"), saved.CustomDateTime);
    }

    [TestMethod]
    public void GetConfiguration_ReturnsConfiguration_WhenExists()
    {
        // Arrange
        var config = new ClockConfiguration
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            CustomDateTime = DateTime.Parse("2025-09-15T14:30"),
        };
        this.context.ClockConfigurations.Add(config);
        this.context.SaveChanges();

        // Act
        var result = this.repository.GetConfiguration();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(config.Id, result.Id);
        Assert.AreEqual(config.CustomDateTime, result.CustomDateTime);
    }

    [TestMethod]
    public void SaveConfiguration_WithNullCustomDateTime_SavesCorrectly()
    {
        // Arrange
        var config = new ClockConfiguration
        {
            CustomDateTime = null,
        };

        // Act
        this.repository.SaveConfiguration(config);

        // Assert
        var saved = this.context.ClockConfigurations.FirstOrDefault();
        Assert.IsNotNull(saved);
        Assert.IsNull(saved.CustomDateTime);
    }

    [TestMethod]
    public void SaveConfiguration_SetsFixedId()
    {
        // Arrange
        var config = new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-01-01T00:00"),
        };

        // Act
        this.repository.SaveConfiguration(config);

        // Assert
        var saved = this.context.ClockConfigurations.FirstOrDefault();
        Assert.IsNotNull(saved);
        Assert.AreEqual(Guid.Parse("00000000-0000-0000-0000-000000000003"), saved.Id);
    }

    [TestMethod]
    public void GetConfiguration_CanBeCalledMultipleTimes()
    {
        // Arrange
        var config = new ClockConfiguration
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            CustomDateTime = DateTime.Parse("2025-09-10T10:00"),
        };
        this.context.ClockConfigurations.Add(config);
        this.context.SaveChanges();

        // Act: Llamar múltiples veces al método
        var result1 = this.repository.GetConfiguration();
        var result2 = this.repository.GetConfiguration();
        var result3 = this.repository.GetConfiguration();

        // Assert: Todas las llamadas deben retornar la misma configuración
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.IsNotNull(result3);
        Assert.AreEqual(config.CustomDateTime, result1.CustomDateTime);
        Assert.AreEqual(config.CustomDateTime, result2.CustomDateTime);
        Assert.AreEqual(config.CustomDateTime, result3.CustomDateTime);
    }

    [TestMethod]
    public void SaveConfiguration_MultipleCalls_OnlyOneConfiguration()
    {
        // Arrange & Act: Guardar múltiples veces
        this.repository.SaveConfiguration(new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-01-01T00:00"),
        });
        this.repository.SaveConfiguration(new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-06-15T12:00"),
        });
        this.repository.SaveConfiguration(new ClockConfiguration
        {
            CustomDateTime = DateTime.Parse("2025-12-31T23:59"),
        });

        // Assert: Debe haber solo una configuración con el último valor
        var allConfigs = this.context.ClockConfigurations.ToList();
        Assert.AreEqual(1, allConfigs.Count);
        Assert.AreEqual(DateTime.Parse("2025-12-31T23:59"), allConfigs[0].CustomDateTime);
    }
}
