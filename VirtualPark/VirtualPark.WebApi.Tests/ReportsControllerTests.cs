// <copyright file="ReportsControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Reports;
using VirtualPark.Domain;
using VirtualPark.DTOs.Reports.Responses;
using VirtualPark.WebApi.Controllers;
using AttractionUsageData = VirtualPark.Application.Reports.AttractionUsageData;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class ReportsControllerTests
{
    private Mock<IReportsService> mockReportsService = null!;
    private ReportsController controller = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockReportsService = new Mock<IReportsService>();
        this.controller = new ReportsController(this.mockReportsService.Object);
    }

    [TestMethod]
    public void GetAttractionUsage_ShouldReturnOkWithReportData()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        var attraction1 = new Attraction("Montaña Rusa", "Emocionante", "10", 2, 100);
        var attraction2 = new Attraction("Casa del Terror", "Escalofriante", "15", 3, 80);

        var reportData = new List<AttractionUsageData>
        {
            new() { Attraction = attraction1, VisitCount = 150 },
            new() { Attraction = attraction2, VisitCount = 200 },
        };

        this.mockReportsService
            .Setup(s => s.GetAttractionUsageReport(startDate, endDate))
            .Returns(reportData);

        // Act
        var result = this.controller.GetAttractionUsage(startDate, endDate);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedData = okResult.Value as List<AttractionUsageReportDto>;
        Assert.IsNotNull(returnedData);
        Assert.AreEqual(2, returnedData.Count);
        Assert.AreEqual(attraction1.Id, returnedData[0].AttractionId);
        Assert.AreEqual("Montaña Rusa", returnedData[0].AttractionName);
        Assert.AreEqual(150, returnedData[0].VisitCount);
        Assert.AreEqual(attraction2.Id, returnedData[1].AttractionId);
        Assert.AreEqual("Casa del Terror", returnedData[1].AttractionName);
        Assert.AreEqual(200, returnedData[1].VisitCount);
    }

    [TestMethod]
    public void GetAttractionUsage_ShouldReturnBadRequest_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = new DateTime(2024, 2, 1);
        var endDate = new DateTime(2024, 1, 1);

        // Act
        var result = this.controller.GetAttractionUsage(startDate, endDate);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual("La fecha de inicio no puede ser mayor a la fecha de fin.", badRequestResult.Value);

        this.mockReportsService.Verify(
            s => s.GetAttractionUsageReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [TestMethod]
    public void GetAttractionUsage_ShouldReturnEmptyList_WhenNoDataExists()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        this.mockReportsService
            .Setup(s => s.GetAttractionUsageReport(startDate, endDate))
            .Returns([]);

        // Act
        var result = this.controller.GetAttractionUsage(startDate, endDate);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedData = okResult.Value as List<AttractionUsageReportDto>;
        Assert.IsNotNull(returnedData);
        Assert.AreEqual(0, returnedData.Count);
    }

    [TestMethod]
    public void GetAttractionUsage_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31);

        this.mockReportsService
            .Setup(s => s.GetAttractionUsageReport(startDate, endDate))
            .Returns([]);

        // Act
        this.controller.GetAttractionUsage(startDate, endDate);

        // Assert
        this.mockReportsService.Verify(
            s => s.GetAttractionUsageReport(startDate, endDate),
            Times.Once);
    }

    [TestMethod]
    public void GetAttractionUsage_ShouldAcceptEqualStartAndEndDates()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);

        var attraction = new Attraction("Carrusel", "Clásico", "5", 1, 50);
        var reportData = new List<AttractionUsageData>
        {
            new() { Attraction = attraction, VisitCount = 50 },
        };

        this.mockReportsService
            .Setup(s => s.GetAttractionUsageReport(date, date))
            .Returns(reportData);

        // Act
        var result = this.controller.GetAttractionUsage(date, date);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedData = okResult.Value as List<AttractionUsageReportDto>;
        Assert.IsNotNull(returnedData);
        Assert.AreEqual(1, returnedData.Count);
    }
}
