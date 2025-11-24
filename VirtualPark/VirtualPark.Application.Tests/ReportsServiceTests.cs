// <copyright file="ReportsServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Reports;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ReportsServiceTests
{
    private Mock<IAttractionAccessRepository> attractionAccessRepoMock = null!;
    private Mock<IAttractionRepository> attractionRepoMock = null!;
    private IReportsService reportsService = null!;

    [TestInitialize]
    public void Setup()
    {
        this.attractionAccessRepoMock = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        this.attractionRepoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);

        this.reportsService = new ReportsService(
            this.attractionAccessRepoMock.Object,
            this.attractionRepoMock.Object);
    }

    [TestMethod]
    public void GetAttractionUsageReport_ReturnsCorrectUsageData()
    {
        // Arrange
        var startDate = new DateTime(2025, 10, 1);
        var endDate = new DateTime(2025, 10, 1).AddDays(1);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), null, startDate, null, EntryMethod.QR)
            {
                AttractionId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            },
            new AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), null, startDate, null, EntryMethod.QR)
            {
                AttractionId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            },
            new AttractionAccess(Guid.NewGuid(), Guid.NewGuid(), null, startDate, null, EntryMethod.QR)
            {
                AttractionId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            },
        };

        var attraction1 = new Attraction("Haunted House", "Scary", "Casa", 15, 20);

        var attraction2 = new Attraction("Haunted House X", "Scary", "Casa", 18, 10);

        this.attractionAccessRepoMock
            .Setup(repo => repo.GetAccessesBetweenDates(startDate, endDate))
            .Returns(accesses);

        this.attractionRepoMock.Setup(repo => repo.GetById(Guid.Parse("00000000-0000-0000-0000-000000000001"))).Returns(attraction1);
        this.attractionRepoMock.Setup(repo => repo.GetById(Guid.Parse("00000000-0000-0000-0000-000000000002"))).Returns(attraction2);

        // Act
        var result = this.reportsService.GetAttractionUsageReport(startDate, endDate).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.First(r => r.Attraction.Id == attraction1.Id).VisitCount);
        Assert.AreSame(attraction1, result.First(r => r.Attraction.Id == attraction1.Id).Attraction);
        Assert.AreEqual(1, result.First(r => r.Attraction.Id == attraction2.Id).VisitCount);
        Assert.AreSame(attraction2, result.First(r => r.Attraction.Id == attraction2.Id).Attraction);
    }
}
