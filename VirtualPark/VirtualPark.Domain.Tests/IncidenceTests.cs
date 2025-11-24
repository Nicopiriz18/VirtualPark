// <copyright file="IncidenceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class IncidenceTests
{
    [TestMethod]
    public void IncidenceConstructor_ShouldSetAllProperties()
    {
        var date = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var attractionId = Guid.NewGuid();

        var incidence = new Incidence("Title", "Description", true, date, attractionId);

        Assert.AreNotEqual(Guid.Empty, incidence.Id);
        Assert.AreEqual("Title", incidence.Title);
        Assert.AreEqual("Description", incidence.Description);
        Assert.IsTrue(incidence.Status);
        Assert.AreEqual(date, incidence.Date);
        Assert.AreEqual(attractionId, incidence.AttractionId);
        Assert.IsNull(incidence.Attraction);
    }

    [TestMethod]
    public void IncidenceCreatedThroughAttraction_ShouldLinkAttractionId()
    {
        var attraction = new Attraction("Roller Coaster 2000", "extreme", "Roller Coaster", 5, 120);
        var date = new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc);

        var incidence = attraction.AddIncidence("Safety issue", "Seat belt loose", false, date);

        Assert.AreEqual(attraction.Id, incidence.AttractionId);
        Assert.AreEqual(1, attraction.Incidences.Count);
    }

    [TestMethod]
    public void CloseIncidence_ShouldSetStatusToFalse()
    {
        var incidence = new Incidence("Title", "Description", true, DateTime.UtcNow, Guid.NewGuid());

        incidence.Close();

        Assert.IsFalse(incidence.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CloseIncidence_AlreadyClosed_ShouldThrowException()
    {
        var incidence = new Incidence("Title", "Description", false, DateTime.UtcNow, Guid.NewGuid());
        incidence.Close();
    }

    [TestMethod]
    public void ReopenIncidence_ShouldSetStatusToTrue()
    {
        var incidence = new Incidence("Title", "Description", false, DateTime.UtcNow, Guid.NewGuid());
        incidence.Reopen();
        Assert.IsTrue(incidence.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ReopenIncidence_AlreadyOpen_ShouldThrowException()
    {
        var incidence = new Incidence("Title", "Description", true, DateTime.UtcNow, Guid.NewGuid());
        incidence.Reopen();
    }
}
