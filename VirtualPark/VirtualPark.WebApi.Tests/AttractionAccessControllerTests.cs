// <copyright file="AttractionAccessControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.AttractionAccess.Requests;
using VirtualPark.DTOs.Attractions.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class AttractionAccessControllerTests
{
    private Mock<IAttractionAccessService> serviceMock = null!;
    private Mock<IAttractionIncidenceService> attractionServiceMock = null!;
    private Mock<IClockService> clockServiceMock = null!;
    private AttractionAccessController controller = null!;
    private readonly Guid attractionId = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        this.serviceMock = new Mock<IAttractionAccessService>();
        this.attractionServiceMock = new Mock<IAttractionIncidenceService>();
        this.clockServiceMock = new Mock<IClockService>();
        this.clockServiceMock.Setup(c => c.GetNow()).Returns(DateTime.Now);
        this.controller = new AttractionAccessController(this.serviceMock.Object, this.attractionServiceMock.Object, this.clockServiceMock.Object);
    }

    [TestMethod]
    public void RegisterAccess_QR_ReturnsOk_AndCallsService()
    {
        // Arrange
        var req = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.QR,
            QrCode = Guid.NewGuid(),
            VisitorId = null,
            VisitDate = DateTime.Today,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        this.serviceMock
            .Setup(s => s.RegisterAccessByQrCode(
                this.attractionId,
                req.QrCode.Value,
                EntryMethod.QR))
            .Verifiable();

        // Act
        var result = this.controller.RegisterAccess(this.attractionId, req) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        this.serviceMock.Verify();
    }

    [TestMethod]
    public void RegisterAccess_QR_MissingQrCode_ReturnsBadRequest()
    {
        // Arrange
        var req = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.QR,
            QrCode = null,
            VisitorId = null,
            VisitDate = DateTime.Today,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        // Act
        var result = this.controller.RegisterAccess(this.attractionId, req) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        this.serviceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void RegisterAccess_NFC_ReturnsOk_AndCallsService()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var req = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.NFC,
            QrCode = null,
            VisitorId = visitorId,
            VisitDate = DateTime.Today,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        this.serviceMock
            .Setup(s => s.RegisterAccessByVisitorId(
                this.attractionId,
                visitorId,
                req.VisitDate,
                EntryMethod.NFC))
            .Verifiable();

        // Act
        var result = this.controller.RegisterAccess(this.attractionId, req) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        this.serviceMock.Verify();
    }

    [TestMethod]
    public void RegisterAccess_NFC_MissingVisitorId_ReturnsBadRequest()
    {
        // Arrange
        var req = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.NFC,
            QrCode = null,
            VisitorId = null,
            VisitDate = DateTime.Today,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        // Act
        var result = this.controller.RegisterAccess(this.attractionId, req) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        this.serviceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void RegisterExit_ReturnsOk_AndCallsService()
    {
        // Arrange
        var visitorId = Guid.NewGuid();

        this.serviceMock
            .Setup(s => s.RegisterExit(this.attractionId, visitorId, It.IsAny<DateTime>()))
            .Verifiable();

        // Act
        var result = this.controller.RegisterExit(this.attractionId, visitorId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        this.serviceMock.Verify();
    }

    [TestMethod]
    public void GetCapacity_ReturnsOk_WithValues()
    {
        // Arrange
        this.serviceMock
            .Setup(s => s.GetAforo(this.attractionId))
            .Returns((AforoActual: 12, CapacidadRestante: 8));

        // Act
        var result = this.controller.GetCapacity(this.attractionId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        // Usar reflexión para acceder a las propiedades del objeto anónimo
        var body = result.Value!;
        var bodyType = body.GetType();
        var currentOccupancy = (int)bodyType.GetProperty("CurrentOccupancy")!.GetValue(body)!;
        var remainingCapacity = (int)bodyType.GetProperty("RemainingCapacity")!.GetValue(body)!;

        Assert.AreEqual(12, currentOccupancy);
        Assert.AreEqual(8, remainingCapacity);
    }

    [TestMethod]
    public void RegisterAccess_ThrowsKeyNotFoundException_FilterHandlesCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var request = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.QR,
            QrCode = Guid.NewGuid(),
            VisitorId = null,
            VisitDate = DateTime.Now,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        this.serviceMock
            .Setup(s => s.RegisterAccessByQrCode(attractionId, request.QrCode.Value, EntryMethod.QR))
            .Throws(new KeyNotFoundException("Atracción no encontrada"));

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(() =>
            this.controller.RegisterAccess(attractionId, request));

        Assert.AreEqual("Atracción no encontrada", exception.Message);
    }

    [TestMethod]
    public void RegisterAccess_ThrowsInvalidOperationException_FilterHandlesCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var request = new RegisterAccessRequestDto
        {
            EntryMethod = EntryMethod.QR,
            QrCode = Guid.NewGuid(),
            VisitorId = null,
            VisitDate = DateTime.Now,
            Type = TicketType.General,
            SpecialEventId = null,
        };

        this.serviceMock
            .Setup(s => s.RegisterAccessByQrCode(attractionId, request.QrCode.Value, EntryMethod.QR))
            .Throws(new InvalidOperationException("No hay capacidad disponible"));

        // Act & Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            this.controller.RegisterAccess(attractionId, request));

        Assert.AreEqual("No hay capacidad disponible", exception.Message);
    }

    [TestMethod]
    public void RegisterExit_ThrowsKeyNotFoundException_FilterHandlesCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();

        this.serviceMock
            .Setup(s => s.RegisterExit(attractionId, visitorId, It.IsAny<DateTime>()))
            .Throws(new KeyNotFoundException("Visitante no encontrado en la atracción"));

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(() =>
            this.controller.RegisterExit(attractionId, visitorId));

        Assert.AreEqual("Visitante no encontrado en la atracción", exception.Message);
    }

    [TestMethod]
    public void RegisterExit_ThrowsInvalidOperationException_FilterHandlesCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();

        this.serviceMock
            .Setup(s => s.RegisterExit(attractionId, visitorId, It.IsAny<DateTime>()))
            .Throws(new InvalidOperationException("El visitante no está registrado como dentro de la atracción"));

        // Act & Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            this.controller.RegisterExit(attractionId, visitorId));

        Assert.AreEqual("El visitante no está registrado como dentro de la atracción", exception.Message);
    }

    [TestMethod]
    public void CreateIncident_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var attraction = new Attraction("Test Attraction", "Test Description", "Extreme", 12, 30);
        var attractionId = attraction.Id;
        var request = new VirtualPark.DTOs.Attractions.Requests.CreateIncidenceRequestDto
        {
            Title = "Avería mecánica",
            Description = "Problema en el motor principal",
            Status = true,
            Date = DateTime.UtcNow,
        };
        var incident = new Incidence("Avería mecánica", "Problema en el motor principal", true, DateTime.UtcNow, attractionId);

        // Set the Attraction property using reflection
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        attractionProperty?.SetValue(incident, attraction);

        this.attractionServiceMock
            .Setup(s => s.AddIncidence(attractionId, request.Title, request.Description, true, It.IsAny<DateTime>()))
            .Returns(incident);

        // Act
        var result = this.controller.CreateIncident(attractionId, request) as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(nameof(this.controller.GetIncidents), result.ActionName);
        var incidenceDto = result.Value as IncidenceDto;
        Assert.IsNotNull(incidenceDto);
        Assert.AreEqual("Avería mecánica", incidenceDto.Title);
    }

    [TestMethod]
    public void CreateIncident_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var request = new VirtualPark.DTOs.Attractions.Requests.CreateIncidenceRequestDto
        {
            Title = "Avería mecánica",
            Description = "Problema en el motor principal",
            Status = true,
            Date = DateTime.UtcNow,
        };

        this.attractionServiceMock
            .Setup(s => s.AddIncidence(attractionId, request.Title, request.Description, true, It.IsAny<DateTime>()))
            .Throws(new KeyNotFoundException("Atracción no encontrada"));

        // Act
        var result = this.controller.CreateIncident(attractionId, request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void CloseIncident_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.CloseIncidence(attractionId, incidentId))
            .Verifiable();

        // Act
        var result = this.controller.UpdateIncident(attractionId, incidentId, "close") as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        this.attractionServiceMock.Verify();

        var body = result.Value as dynamic;
        Assert.IsNotNull(body);
        Assert.AreEqual("Incidencia cerrada con éxito", body!.GetType().GetProperty("Message")!.GetValue(body));
    }

    [TestMethod]
    public void CloseIncident_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.CloseIncidence(attractionId, incidentId))
            .Throws(new KeyNotFoundException("Incidencia no encontrada"));

        // Act
        var result = this.controller.UpdateIncident(attractionId, incidentId, "close") as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void ReopenIncident_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.ReopenIncidence(attractionId, incidentId))
            .Verifiable();

        // Act
        var result = this.controller.UpdateIncident(attractionId, incidentId, "reopen") as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        this.attractionServiceMock.Verify();

        var body = result.Value as dynamic;
        Assert.IsNotNull(body);
        Assert.AreEqual("Incidencia reabierta con éxito", body!.GetType().GetProperty("Message")!.GetValue(body));
    }

    [TestMethod]
    public void ReopenIncident_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.ReopenIncidence(attractionId, incidentId))
            .Throws(new KeyNotFoundException("Incidencia no encontrada"));

        // Act
        var result = this.controller.UpdateIncident(attractionId, incidentId, "reopen") as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void GetIncident_ReturnsOk_WhenIncidentExists()
    {
        // Arrange
        var attraction = new Attraction("Test Attraction", "Test Description", "Extreme", 12, 30);
        var attractionId = attraction.Id;
        var incidentId = Guid.NewGuid();
        var incident = new Incidence("Test Incident", "Test Description", true, DateTime.UtcNow, attractionId);

        // Set the Attraction property using reflection
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        attractionProperty?.SetValue(incident, attraction);

        var incidents = new List<Incidence> { incident };

        this.attractionServiceMock
            .Setup(s => s.GetIncidences(attractionId))
            .Returns(incidents);

        // Act
        var result = this.controller.GetIncidents(attractionId, incident.Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var incidenceDto = result.Value as IncidenceDto;
        Assert.IsNotNull(incidenceDto);
        Assert.AreEqual("Test Incident", incidenceDto.Title);
    }

    [TestMethod]
    public void GetIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();
        var incidents = new List<Incidence>();

        this.attractionServiceMock
            .Setup(s => s.GetIncidences(attractionId))
            .Returns(incidents);

        // Act
        var result = this.controller.GetIncidents(attractionId, incidentId) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void GetIncident_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.GetIncidences(attractionId))
            .Throws(new KeyNotFoundException("Atracción no encontrada"));

        // Act
        var result = this.controller.GetIncidents(attractionId, incidentId) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void GetIncidents_ReturnsOk_WithIncidentsList()
    {
        // Arrange
        var attraction = new Attraction("Test Attraction", "Test Description", "Extreme", 12, 30);
        var attractionId = attraction.Id;
        var incident1 = new Incidence("Incident 1", "Description 1", true, DateTime.UtcNow, attractionId);
        var incident2 = new Incidence("Incident 2", "Description 2", false, DateTime.UtcNow, attractionId);

        // Set the Attraction property using reflection
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        attractionProperty?.SetValue(incident1, attraction);
        attractionProperty?.SetValue(incident2, attraction);

        var incidents = new List<Incidence> { incident1, incident2 };

        this.attractionServiceMock
            .Setup(s => s.GetIncidences(attractionId))
            .Returns(incidents);

        // Act
        var result = this.controller.GetIncidents(attractionId, null) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var incidenceDtos = result.Value as IEnumerable<IncidenceDto>;
        Assert.IsNotNull(incidenceDtos);
        Assert.AreEqual(2, incidenceDtos.Count());
        Assert.AreEqual("Incident 1", incidenceDtos.First().Title);
    }

    [TestMethod]
    public void GetIncidents_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.GetIncidences(attractionId))
            .Throws(new KeyNotFoundException("Atracción no encontrada"));

        // Act
        var result = this.controller.GetIncidents(attractionId, null) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void GetAttractionStatus_ReturnsOk_WithOutOfServiceTrue_WhenActiveIncidentsExist()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var activeIncident = new Incidence("Active Incident", "Description", true, DateTime.UtcNow, attractionId);
        var activeIncidents = new List<Incidence> { activeIncident };

        this.attractionServiceMock
            .Setup(s => s.GetActiveIncidences(attractionId))
            .Returns(activeIncidents);

        // Act
        var result = this.controller.GetAttractionStatus(attractionId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var body = result.Value!;
        var bodyType = body.GetType();
        var isOutOfService = (bool)bodyType.GetProperty("IsOutOfService")!.GetValue(body)!;
        var returnedAttractionId = (Guid)bodyType.GetProperty("AttractionId")!.GetValue(body)!;

        Assert.IsTrue(isOutOfService);
        Assert.AreEqual(attractionId, returnedAttractionId);
    }

    [TestMethod]
    public void GetAttractionStatus_ReturnsOk_WithOutOfServiceFalse_WhenNoActiveIncidents()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var activeIncidents = new List<Incidence>(); // empty list

        this.attractionServiceMock
            .Setup(s => s.GetActiveIncidences(attractionId))
            .Returns(activeIncidents);

        // Act
        var result = this.controller.GetAttractionStatus(attractionId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var body = result.Value!;
        var bodyType = body.GetType();
        var isOutOfService = (bool)bodyType.GetProperty("IsOutOfService")!.GetValue(body)!;

        Assert.IsFalse(isOutOfService);
    }

    [TestMethod]
    public void GetAttractionStatus_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var attractionId = Guid.NewGuid();

        this.attractionServiceMock
            .Setup(s => s.GetActiveIncidences(attractionId))
            .Throws(new KeyNotFoundException("Atracción no encontrada"));

        // Act
        var result = this.controller.GetAttractionStatus(attractionId) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
}
