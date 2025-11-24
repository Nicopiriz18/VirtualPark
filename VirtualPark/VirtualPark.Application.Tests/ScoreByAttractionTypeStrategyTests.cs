// <copyright file="ScoreByAttractionTypeStrategyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ScoreByAttractionTypeStrategyTests
{
    private ScoreByAttractionTypeStrategy strategy = null!;
    private Visitor visitor = null!;
    private List<AttractionAccess> emptyAccesses = null!;

    [TestInitialize]
    public void Setup()
    {
        this.strategy = new ScoreByAttractionTypeStrategy();
        this.visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        this.emptyAccesses = [];
    }

    [TestMethod]
    public void Name_ReturnsCorrectStrategyName()
    {
        var name = this.strategy.Name;

        Assert.AreEqual("ScoreByAttractionType", name);
    }

    [TestMethod]
    public void CalculatePoints_RollerCoaster_Returns100Points()
    {
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_HauntedHouse_Returns80Points()
    {
        var attraction = new Attraction("Haunted Mansion", "Scary experience", "HauntedHouse", 10, 15);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(80, points);
    }

    [TestMethod]
    public void CalculatePoints_WaterRide_Returns70Points()
    {
        var attraction = new Attraction("Splash Mountain", "Water adventure", "WaterRide", 8, 25);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(70, points);
    }

    [TestMethod]
    public void CalculatePoints_Carousel_Returns50Points()
    {
        var attraction = new Attraction("Magic Carousel", "Classic carousel", "Carousel", 0, 30);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(50, points);
    }

    [TestMethod]
    public void CalculatePoints_FerrisWheel_Returns60Points()
    {
        var attraction = new Attraction("Sky Wheel", "Giant wheel", "FerrisWheel", 0, 40);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(60, points);
    }

    [TestMethod]
    public void CalculatePoints_BumperCars_Returns40Points()
    {
        var attraction = new Attraction("Bumper Arena", "Bumper cars", "BumperCars", 6, 20);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(40, points);
    }

    [TestMethod]
    public void CalculatePoints_UnknownType_Returns30Points()
    {
        var attraction = new Attraction("Mystery Ride", "Unknown type", "UnknownType", 5, 10);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(30, points);
    }

    [TestMethod]
    public void CalculatePoints_CaseInsensitive_RollerCoaster()
    {
        var attraction1 = new Attraction("Ride 1", "Test", "rollercoaster", 12, 20);
        var attraction2 = new Attraction("Ride 2", "Test", "ROLLERCOASTER", 12, 20);
        var attraction3 = new Attraction("Ride 3", "Test", "RoLLeRcOaStEr", 12, 20);

        var points1 = this.strategy.CalculatePoints(attraction1, this.visitor, this.emptyAccesses, null);
        var points2 = this.strategy.CalculatePoints(attraction2, this.visitor, this.emptyAccesses, null);
        var points3 = this.strategy.CalculatePoints(attraction3, this.visitor, this.emptyAccesses, null);

        Assert.AreEqual(100, points1);
        Assert.AreEqual(100, points2);
        Assert.AreEqual(100, points3);
    }

    [TestMethod]
    public void CalculatePoints_IgnoresOtherParameters()
    {
        var attraction = new Attraction("Test Ride", "Test", "RollerCoaster", 12, 20);
        var accesses = new List<AttractionAccess>
        {
            new AttractionAccess(Guid.NewGuid(), this.visitor.Id, Guid.NewGuid(), DateTime.Now, null, EntryMethod.QR),
        };
        var specialEvent = new Domain.SpecialEvent("Test Event", DateTime.Today, 100, 10);

        var points = this.strategy.CalculatePoints(attraction, this.visitor, accesses, specialEvent);

        Assert.AreEqual(100, points);
    }
}
