// <copyright file="ClockServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ClockServiceTests
{
    private Mock<IClockRepository> mockRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockRepository = new Mock<IClockRepository>();
    }

    [TestMethod]
    public void GetNow_ReturnsSystemTime_IfNotConfigured()
    {
        // Arrange: El repositorio retorna null (no hay configuración personalizada)
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns((ClockConfiguration?)null);

        var service = new ClockService(this.mockRepository.Object);

        // Act
        var result = service.GetNow();

        // Assert: Debería retornar la hora del sistema (aproximadamente DateTime.Now)
        Assert.IsTrue((DateTime.Now - result).TotalSeconds < 2);
    }

    [TestMethod]
    public void GetNow_ReturnsSystemTime_WhenCustomDateTimeIsNull()
    {
        // Arrange: El repositorio retorna una configuración con CustomDateTime = null
        var config = new ClockConfiguration { Id = Guid.NewGuid(), CustomDateTime = null };
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns(config);

        var service = new ClockService(this.mockRepository.Object);

        // Act
        var result = service.GetNow();

        // Assert
        Assert.IsTrue((DateTime.Now - result).TotalSeconds < 2);
    }

    [TestMethod]
    public void GetNow_ReturnsCustomTime_WhenConfigured()
    {
        // Arrange: El repositorio retorna una configuración con un CustomDateTime específico
        var customDate = DateTime.Parse("2025-09-10T10:00");
        var config = new ClockConfiguration { Id = Guid.NewGuid(), CustomDateTime = customDate };
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns(config);

        var service = new ClockService(this.mockRepository.Object);

        // Act
        var result = service.GetNow();

        // Assert
        Assert.AreEqual(customDate, result);
    }

    [TestMethod]
    public void SetNow_OverridesSystemTime()
    {
        // Arrange: Configuración inicial sin tiempo personalizado
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns((ClockConfiguration?)null);

        var service = new ClockService(this.mockRepository.Object);
        var customDate = DateTime.Parse("2025-09-10T10:00");

        // Act
        service.SetNow(customDate);
        var result = service.GetNow();

        // Assert
        Assert.AreEqual(customDate, result);
        this.mockRepository.Verify(
            r => r.SaveConfiguration(It.Is<ClockConfiguration>(
            c => c.CustomDateTime == customDate)), Times.Once);
    }

    [TestMethod]
    public void SetNow_PersistsToDatabase()
    {
        // Arrange
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns((ClockConfiguration?)null);

        var service = new ClockService(this.mockRepository.Object);
        var customDate = DateTime.Parse("2025-12-25T23:59:59");

        // Act
        service.SetNow(customDate);

        // Assert: Verificar que se llamó a SaveConfiguration con el valor correcto
        this.mockRepository.Verify(
            r => r.SaveConfiguration(It.Is<ClockConfiguration>(
            c => c.CustomDateTime == customDate)), Times.Once);
    }

    [TestMethod]
    public void GetNow_LoadsConfigurationFromDatabase_OnFirstCall()
    {
        // Arrange
        var customDate = DateTime.Parse("2025-01-01T00:00:00");
        var config = new ClockConfiguration { Id = Guid.NewGuid(), CustomDateTime = customDate };
        this.mockRepository
            .Setup(r => r.GetConfiguration())
            .Returns(config);

        var service = new ClockService(this.mockRepository.Object);

        // Act: Primera llamada a GetNow dispara la carga de configuración
        var result = service.GetNow();

        // Assert
        this.mockRepository.Verify(r => r.GetConfiguration(), Times.Once);
        Assert.AreEqual(customDate, result);

        // Segunda llamada no debe consultar la BD nuevamente (está en caché)
        service.GetNow();
        this.mockRepository.Verify(r => r.GetConfiguration(), Times.Once); // Sigue siendo Once
    }
}
