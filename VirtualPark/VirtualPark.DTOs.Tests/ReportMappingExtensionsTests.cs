// <copyright file="ReportMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Reports;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class ReportMappingExtensionsTests
{
    [TestMethod]
    public void ToAttractionUsageReportDto_ShouldMapToReportDtoCorrectly()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var startDate = new DateTime(2025, 10, 1);
        var endDate = new DateTime(2025, 10, 31);
        var peakDay = new DateTime(2025, 10, 15);

        // Act
        var dto = ReportMappingExtensions.ToAttractionUsageReportDto(
            attractionId,
            "Roller Coaster",
            "Extreme",
            500,
            startDate,
            endDate,
            50,
            75.5m,
            16,
            45,
            peakDay);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attractionId, dto.AttractionId);
        Assert.AreEqual("Roller Coaster", dto.AttractionName);
        Assert.AreEqual("Extreme", dto.AttractionType);
        Assert.AreEqual(500, dto.VisitCount);
        Assert.AreEqual(startDate, dto.StartDate);
        Assert.AreEqual(endDate, dto.EndDate);
        Assert.AreEqual(50, dto.Capacity);
        Assert.AreEqual(75.5m, dto.OccupancyRate);
        Assert.AreEqual(16, dto.AverageVisitsPerDay);
        Assert.AreEqual(45, dto.PeakDayVisits);
        Assert.AreEqual(peakDay, dto.PeakDay);
    }

    [TestMethod]
    public void ToAttractionUsageReportDto_ShouldHandleZeroCapacity()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var startDate = new DateTime(2025, 9, 1);
        var endDate = new DateTime(2025, 9, 30);

        // Act
        var dto = ReportMappingExtensions.ToAttractionUsageReportDto(
            attractionId,
            "Water Slide",
            "Water",
            100,
            startDate,
            endDate,
            0,
            0,
            3,
            0,
            null);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(0, dto.Capacity);
        Assert.AreEqual(0, dto.OccupancyRate);
        Assert.IsNull(dto.PeakDay);
    }

    [TestMethod]
    public void ToVisitorActivityReportDto_ShouldMapToReportDtoCorrectly()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var startDate = new DateTime(2025, 10, 1);
        var endDate = new DateTime(2025, 10, 31);
        var lastVisitDate = new DateTime(2025, 10, 25);
        var mostVisitedAttractions = new List<string> { "Roller Coaster", "Ferris Wheel", "Haunted House" };

        // Act
        var dto = ReportMappingExtensions.ToVisitorActivityReportDto(
            visitorId,
            "John Doe",
            "john@example.com",
            MembershipLevel.Premium,
            15,
            45,
            1200,
            startDate,
            endDate,
            10,
            lastVisitDate,
            3,
            mostVisitedAttractions);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitorId, dto.VisitorId);
        Assert.AreEqual("John Doe", dto.VisitorName);
        Assert.AreEqual("john@example.com", dto.Email);
        Assert.AreEqual(MembershipLevel.Premium, dto.MembershipLevel);
        Assert.AreEqual(15, dto.TotalVisits);
        Assert.AreEqual(45, dto.TotalAttractionAccesses);
        Assert.AreEqual(1200, dto.TotalPoints);
        Assert.AreEqual(startDate, dto.StartDate);
        Assert.AreEqual(endDate, dto.EndDate);
        Assert.AreEqual(10, dto.UniqueAttractionsVisited);
        Assert.AreEqual(lastVisitDate, dto.LastVisitDate);
        Assert.AreEqual(3, dto.TicketsPurchased);
        Assert.IsNotNull(dto.MostVisitedAttractions);
        Assert.AreEqual(3, dto.MostVisitedAttractions.Count);
    }

    [TestMethod]
    public void ToVisitorActivityReportDto_ShouldHandleNullOptionalFields()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var startDate = new DateTime(2025, 8, 1);
        var endDate = new DateTime(2025, 8, 31);

        // Act
        var dto = ReportMappingExtensions.ToVisitorActivityReportDto(
            visitorId,
            "Jane Smith",
            "jane@example.com",
            MembershipLevel.Standard,
            5,
            10,
            300,
            startDate,
            endDate);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Jane Smith", dto.VisitorName);
        Assert.AreEqual(0, dto.UniqueAttractionsVisited);
        Assert.IsNull(dto.LastVisitDate);
        Assert.AreEqual(0, dto.TicketsPurchased);
        Assert.IsNull(dto.MostVisitedAttractions);
    }

    [TestMethod]
    public void ToVisitorActivityReportDto_ShouldHandleDifferentMembershipLevels()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var startDate = new DateTime(2025, 7, 1);
        var endDate = new DateTime(2025, 7, 31);

        // Act
        var dto = ReportMappingExtensions.ToVisitorActivityReportDto(
            visitorId,
            "Bob Johnson",
            "bob@example.com",
            MembershipLevel.VIP,
            20,
            60,
            2000,
            startDate,
            endDate);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(MembershipLevel.VIP, dto.MembershipLevel);
    }

    [TestMethod]
    public void ToAttractionUsageReportDto_ShouldHandleNullPeakDay()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var startDate = new DateTime(2025, 6, 1);
        var endDate = new DateTime(2025, 6, 30);

        // Act
        var dto = ReportMappingExtensions.ToAttractionUsageReportDto(
            attractionId,
            "Bumper Cars",
            "Family",
            250,
            startDate,
            endDate,
            40,
            50.0m,
            8,
            0,
            null);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Bumper Cars", dto.AttractionName);
        Assert.IsNull(dto.PeakDay);
        Assert.AreEqual(0, dto.PeakDayVisits);
    }

    [TestMethod]
    public void ToVisitorActivityReportDto_ShouldHandleEmptyMostVisitedAttractions()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var startDate = new DateTime(2025, 5, 1);
        var endDate = new DateTime(2025, 5, 31);
        var emptyList = new List<string>();

        // Act
        var dto = ReportMappingExtensions.ToVisitorActivityReportDto(
            visitorId,
            "Alice Williams",
            "alice@example.com",
            MembershipLevel.Standard,
            1,
            2,
            50,
            startDate,
            endDate,
            1,
            null,
            1,
            emptyList);

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsNotNull(dto.MostVisitedAttractions);
        Assert.AreEqual(0, dto.MostVisitedAttractions.Count);
    }
}
