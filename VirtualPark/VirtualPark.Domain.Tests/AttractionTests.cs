// <copyright file="AttractionTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class AttractionTests
{
    [TestMethod]
    public void AttractionConstructorTestAll()
    {
        var attraction = new Attraction("Roller Coaster 2000", "extreme", "Roller Coaster", 5, 120);
        Assert.AreEqual("Roller Coaster 2000", attraction.Name);
        Assert.AreEqual("extreme", attraction.Description);
        Assert.AreEqual("Roller Coaster", attraction.Type);
        Assert.AreEqual(5, attraction.MinAge);
        Assert.AreEqual(120, attraction.Capacity);
        Assert.AreNotEqual(Guid.Empty, attraction.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AttractionConstructorTestNullName()
    {
        _ = new Attraction(null!, "extreme", "Roller Coaster", 5, 120);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AttractionNegativeAgeTest()
    {
        _ = new Attraction("Simulador VR", "Simulador VR inmersivo", "Simulador", -1, 20);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AttractionNegativeCapacityTest()
    {
        _ = new Attraction("Simulador VR", "Simulador VR inmersivo", "Simulador", 5, -20);
    }

    [TestMethod]
    public void ModifyAttractionTest()
    {
        var attraction = new Attraction("Simulador VR", "Simulador VR inmersivo", "Simulador", 5, 20);
        attraction.ModifyAttraction("Simulador AR", "Simulador AR inmersivo", "Simulador Avanzado", 10, 30);
        Assert.AreEqual("Simulador AR", attraction.Name);
        Assert.AreEqual("Simulador AR inmersivo", attraction.Description);
        Assert.AreEqual("Simulador Avanzado", attraction.Type);
        Assert.AreEqual(10, attraction.MinAge);
        Assert.AreEqual(30, attraction.Capacity);
    }

    [TestMethod]
    public void AddIncidenceTest()
    {
        var attraction = new Attraction("Roller Coaster 2000", "extreme", "Roller Coaster", 5, 120);
        var incidence = attraction.AddIncidence("Title", "Description", true, DateTime.UtcNow);
        Assert.AreEqual(1, attraction.Incidences.Count);
        Assert.AreEqual(incidence, attraction.Incidences.First());
    }

    [TestMethod]
    public void RemoveIncidenceTest()
    {
        var attraction = new Attraction("Roller Coaster 2000", "extreme", "Roller Coaster", 5, 120);
        var incidence = attraction.AddIncidence("Title", "Description", true, DateTime.UtcNow);
        Assert.AreEqual(1, attraction.Incidences.Count);
        attraction.RemoveIncidence(incidence.Id);
        Assert.AreEqual(0, attraction.Incidences.Count);
    }

    [TestMethod]
    public void RemoveIncidence_WhenIdNotFound_ShouldLeaveCollectionUnchanged()
    {
        var attraction = new Attraction("Roller Coaster 2000", "extreme", "Roller Coaster", 5, 120);
        _ = attraction.AddIncidence("Title", "Description", true, DateTime.UtcNow);
        var beforeRemoval = attraction.Incidences.First();

        attraction.RemoveIncidence(Guid.NewGuid());

        Assert.AreEqual(1, attraction.Incidences.Count);
        Assert.AreEqual(beforeRemoval, attraction.Incidences.First());
    }
}
