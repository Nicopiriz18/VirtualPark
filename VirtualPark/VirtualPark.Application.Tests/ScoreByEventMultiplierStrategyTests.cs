// <copyright file="ScoreByEventMultiplierStrategyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ScoreByEventMultiplierStrategyTests
{
    private ScoreByEventMultiplierStrategy strategy = null!;
    private Visitor visitor = null!;
    private List<AttractionAccess> emptyAccesses = null!;

    [TestInitialize]
    public void Setup()
    {
        this.strategy = new ScoreByEventMultiplierStrategy();
        this.visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        this.emptyAccesses = [];
    }

    [TestMethod]
    public void Name_ReturnsCorrectStrategyName()
    {
        var name = this.strategy.Name;

        Assert.AreEqual("ScoreByEventMultiplier", name);
    }

    [TestMethod]
    public void CalculatePoints_NoActiveEvent_Returns50Points()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(50, points);
    }

    [TestMethod]
    public void CalculatePoints_WithEventButAttractionNotIncluded_Returns50Points()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var specialEvent = new Domain.SpecialEvent("Halloween Night", DateTime.Today, 100, 20);
        var otherAttraction = new Attraction("Haunted House", "Scary", "HauntedHouse", 10, 15);
        specialEvent.AddAttraction(otherAttraction);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, specialEvent);

        Assert.AreEqual(50, points);
    }

    [TestMethod]
    public void CalculatePoints_WithEventAndAttractionIncluded_Returns100Points()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var specialEvent = new Domain.SpecialEvent("Halloween Night", DateTime.Today, 100, 20);
        specialEvent.AddAttraction(attraction);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, specialEvent);

        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_WithEventAndMultipleAttractionsIncluded_AppliesMultiplier()
    {
        var attraction1 = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var attraction2 = new Attraction("Haunted House", "Scary", "HauntedHouse", 10, 15);
        var specialEvent = new Domain.SpecialEvent("Halloween Night", DateTime.Today, 100, 20);
        specialEvent.AddAttraction(attraction1);
        specialEvent.AddAttraction(attraction2);

        var points1 = this.strategy.CalculatePoints(attraction1, this.visitor, this.emptyAccesses, specialEvent);
        var points2 = this.strategy.CalculatePoints(attraction2, this.visitor, this.emptyAccesses, specialEvent);

        Assert.AreEqual(100, points1);
        Assert.AreEqual(100, points2);
    }

    [TestMethod]
    public void CalculatePoints_EventWithNoAttractions_Returns50Points()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var specialEvent = new Domain.SpecialEvent("Empty Event", DateTime.Today, 100, 20);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, specialEvent);

        Assert.AreEqual(50, points);
    }

    [TestMethod]
    public void CalculatePoints_IgnoresVisitorAndAccessesParameters()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var specialEvent = new Domain.SpecialEvent("Halloween Night", DateTime.Today, 100, 20);
        specialEvent.AddAttraction(attraction);

        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(Guid.NewGuid(), this.visitor.Id, Guid.NewGuid(), DateTime.Now, null, EntryMethod.QR),
            new AttractionAccess(Guid.NewGuid(), this.visitor.Id, Guid.NewGuid(), DateTime.Now, null, EntryMethod.NFC),
        };

        var points = this.strategy.CalculatePoints(attraction, this.visitor, accesses, specialEvent);

        Assert.AreEqual(100, points);
    }
}
