// <copyright file="SpecialEventTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class SpecialEventTests
{
    private SpecialEvent? specialEvent;
    private List<Attraction>? attractions;

    [TestInitialize]
    public void Setup()
    {
        this.attractions =
        [
            new Attraction("Roller Coaster", "An exciting roller coaster ride", "Roller Coaster", 12, 24),
            new Attraction("Ferris Wheel", "A relaxing ride with a great view", "Ferris Wheel", 0, 40)
        ];

        this.specialEvent = new SpecialEvent("Halloween", DateTime.Now, 100, 50.0m);
    }

    [TestMethod]
    public void SpecialEventConstructorTestAll()
    {
        Assert.IsNotNull(this.specialEvent.Id);
        Assert.AreEqual("Halloween", this.specialEvent.Name);
        Assert.AreEqual(DateTime.Now.Date, this.specialEvent.Date.Date);
        Assert.AreEqual(100, this.specialEvent.MaxCapacity);
        Assert.AreEqual(50.0m, this.specialEvent.AdditionalCost);
    }

    [TestMethod]
    public void AddAttractionToSpecialEventTest()
    {
        Assert.IsNotNull(this.specialEvent);
        Assert.IsNotNull(this.attractions);

        foreach (var attraction in this.attractions)
        {
            this.specialEvent.AddAttraction(attraction);
        }

        Assert.AreEqual(2, this.specialEvent.Attractions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddAttraction_Null_ThrowsException()
    {
        this.specialEvent?.AddAttraction(null!);
    }

    [TestMethod]
    public void RemoveAttraction_ExistingAttraction_IsRemoved()
    {
        Assert.IsNotNull(this.specialEvent);
        Assert.IsNotNull(this.attractions);

        foreach (var attraction in this.attractions)
        {
            this.specialEvent.AddAttraction(attraction);
        }

        var attractionToRemove = this.attractions[0];
        this.specialEvent.RemoveAttraction(attractionToRemove.Id);

        Assert.AreEqual(1, this.specialEvent.Attractions.Count);
        Assert.IsFalse(this.specialEvent.Attractions.Any(a => a.Id == attractionToRemove.Id));
    }

    [TestMethod]
    public void RemoveAttraction_NonExistingAttraction_DoesNothing()
    {
        Assert.IsNotNull(this.specialEvent);
        Assert.IsNotNull(this.attractions);

        foreach (var attraction in this.attractions)
        {
            this.specialEvent.AddAttraction(attraction);
        }

        var nonExistingId = Guid.NewGuid();
        this.specialEvent.RemoveAttraction(nonExistingId);

        Assert.AreEqual(2, this.specialEvent.Attractions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SpecialEventConstructorTestNullName()
    {
        if (this.attractions != null)
        {
            _ = new SpecialEvent(null!, DateTime.Now, 100, 50.0m);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SpecialEventConstructorMaxCapacityZeroTest()
    {
        if (this.attractions != null)
        {
            _ = new SpecialEvent("Halloween", DateTime.Now, 0, 50.0m);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SpecialEventConstructorMaxCapacityNegativeTest()
    {
        if (this.attractions != null)
        {
            _ = new SpecialEvent("Halloween", DateTime.Now, -10, 50.0m);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SpecialEventConstructorAdditionalCostNegativeTest()
    {
        if (this.attractions != null)
        {
            _ = new SpecialEvent("Halloween", DateTime.Now, 100, -50.0m);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SpecialEventConstructorDateInThePastTest()
    {
        if (this.attractions != null)
        {
            _ = new SpecialEvent("Halloween", DateTime.Now.AddDays(-1), 100, 50.0m);
        }
    }
}
