// <copyright file="ScoreByComboStrategyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ScoreByComboStrategyTests
{
    private ScoreByComboStrategy strategy = null!;
    private Visitor visitor = null!;
    private Guid visitorId;

    [TestInitialize]
    public void Setup()
    {
        this.strategy = new ScoreByComboStrategy();
        this.visitorId = Guid.NewGuid();
        this.visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
    }

    [TestMethod]
    public void Name_ReturnsCorrectStrategyName()
    {
        var name = this.strategy.Name;

        Assert.AreEqual("ScoreByCombo", name);
    }

    [TestMethod]
    public void CalculatePoints_NoAccesses_Returns75Points()
    {
        var attraction = new Attraction("Ride 1", "Test", "RollerCoaster", 10, 20);
        var accesses = new List<AttractionAccess>();

        var points = this.strategy.CalculatePoints(attraction, this.visitor, accesses, null);

        Assert.AreEqual(75, points);
    }

    [TestMethod]
    public void CalculatePoints_FirstAttractionOnly_Returns75Points()
    {
        var attraction = new Attraction("Ride 1", "Test", "RollerCoaster", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);
        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction.Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR),
        };

        var points = this.strategy.CalculatePoints(attraction, this.visitor, accesses, null);

        Assert.AreEqual(75, points);
    }

    [TestMethod]
    public void CalculatePoints_TwoDifferentAttractionsWithin30Minutes_Returns100Points()
    {
        var attraction1Id = Guid.NewGuid();
        var attraction2Id = Guid.NewGuid();
        var attraction2 = new Attraction("Ride 2", "Test", "Carousel", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction1Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-15), null, EntryMethod.QR),
            new AttractionAccess(attraction2Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction2, this.visitor, accesses, null);

        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_ThreeDifferentAttractionsWithin30Minutes_Returns125Points()
    {
        var attraction1Id = Guid.NewGuid();
        var attraction2Id = Guid.NewGuid();
        var attraction3Id = Guid.NewGuid();
        var attraction3 = new Attraction("Ride 3", "Test", "WaterRide", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction1Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-25), null, EntryMethod.QR),
            new AttractionAccess(attraction2Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-10), null, EntryMethod.QR),
            new AttractionAccess(attraction3Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction3, this.visitor, accesses, null);

        Assert.AreEqual(125, points);
    }

    [TestMethod]
    public void CalculatePoints_AccessesOutside30MinutesWindow_DoNotCount()
    {
        var attraction1Id = Guid.NewGuid();
        var attraction2Id = Guid.NewGuid();
        var attraction2 = new Attraction("Ride 2", "Test", "Carousel", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction1Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-35), null, EntryMethod.QR), // Outside window
            new AttractionAccess(attraction2Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction2, this.visitor, accesses, null);

        Assert.AreEqual(75, points);
    }

    [TestMethod]
    public void CalculatePoints_SameAttractionTwice_CountsAsOne()
    {
        var attractionId = Guid.NewGuid();
        var attraction = new Attraction("Ride 1", "Test", "RollerCoaster", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attractionId, this.visitorId, Guid.NewGuid(), now.AddMinutes(-20), null, EntryMethod.QR),
            new AttractionAccess(attractionId, this.visitorId, Guid.NewGuid(), now.AddMinutes(-10), null, EntryMethod.QR),
            new AttractionAccess(attractionId, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction, this.visitor, accesses, null);

        Assert.AreEqual(75, points);
    }

    [TestMethod]
    public void CalculatePoints_ExactlyAt30MinuteBoundary_IsIncluded()
    {
        var attraction1Id = Guid.NewGuid();
        var attraction2Id = Guid.NewGuid();
        var attraction2 = new Attraction("Ride 2", "Test", "Carousel", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction1Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-30), null, EntryMethod.QR), // Exactly at boundary
            new AttractionAccess(attraction2Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction2, this.visitor, accesses, null);

        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_MixedAccessesInsideAndOutsideWindow_OnlyCountsInside()
    {
        var attraction1Id = Guid.NewGuid();
        var attraction2Id = Guid.NewGuid();
        var attraction3Id = Guid.NewGuid();
        var attraction4Id = Guid.NewGuid();
        var attraction4 = new Attraction("Ride 4", "Test", "BumperCars", 10, 20);
        var now = new DateTime(2025, 10, 2, 14, 0, 0);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(attraction1Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-45), null, EntryMethod.QR), // Outside
            new AttractionAccess(attraction2Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-25), null, EntryMethod.QR), // Inside
            new AttractionAccess(attraction3Id, this.visitorId, Guid.NewGuid(), now.AddMinutes(-5), null, EntryMethod.QR),  // Inside
            new AttractionAccess(attraction4Id, this.visitorId, Guid.NewGuid(), now, null, EntryMethod.QR), // Current
        };

        var points = this.strategy.CalculatePoints(attraction4, this.visitor, accesses, null);

        Assert.AreEqual(125, points);
    }
}
