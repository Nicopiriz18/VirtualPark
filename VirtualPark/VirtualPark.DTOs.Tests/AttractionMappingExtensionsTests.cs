// <copyright file="AttractionMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Attractions;
using VirtualPark.DTOs.Attractions.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class AttractionMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ShouldMapAttractionToDtoCorrectly()
    {
        // Arrange
        var attraction = new Attraction(
            "Roller Coaster",
            "Exciting roller coaster ride",
            "Extreme",
            12,
            50);

        // Act
        var dto = attraction.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attraction.Id, dto.Id);
        Assert.AreEqual("Roller Coaster", dto.Name);
        Assert.AreEqual("Exciting roller coaster ride", dto.Description);
        Assert.AreEqual("Extreme", dto.Type);
        Assert.AreEqual(12, dto.MinAge);
        Assert.AreEqual(50, dto.Capacity);
    }

    [TestMethod]
    public void ToDetailDto_ShouldMapAttractionToDetailDtoWithIncidences()
    {
        // Arrange
        var attraction = new Attraction(
            "Ferris Wheel",
            "Giant ferris wheel",
            "Family",
            5,
            100);
        attraction.AddIncidence("Title1", "Description1", true, DateTime.Now);
        attraction.AddIncidence("Title2", "Description2", false, DateTime.Now);

        // Set the Attraction property on each incidence using reflection
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        foreach (var incidence in attraction.Incidences)
        {
            attractionProperty?.SetValue(incidence, attraction);
        }

        // Act
        var dto = attraction.ToDetailDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attraction.Id, dto.Id);
        Assert.AreEqual("Ferris Wheel", dto.Name);
        Assert.AreEqual(2, dto.TotalIncidences);
        Assert.AreEqual(1, dto.ActiveIncidences);
        Assert.AreEqual(2, dto.Incidences.Count);
    }

    [TestMethod]
    public void ToListDto_ShouldMapAttractionToListDtoCorrectly()
    {
        // Arrange
        var attraction = new Attraction(
            "Water Slide",
            "Fun water slide",
            "Water",
            8,
            30);
        attraction.AddIncidence("Active Issue", "Description", true, DateTime.Now);

        // Act
        var dto = attraction.ToListDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(attraction.Id, dto.Id);
        Assert.AreEqual("Water Slide", dto.Name);
        Assert.AreEqual("Water", dto.Type);
        Assert.AreEqual(8, dto.MinAge);
        Assert.AreEqual(30, dto.Capacity);
        Assert.IsTrue(dto.HasActiveIncidences);
    }

    [TestMethod]
    public void ToDto_Incidence_ShouldMapIncidenceToDtoCorrectly()
    {
        // Arrange
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Extreme", 12, 30);
        var date = new DateTime(2025, 10, 6);
        var incidence = new Incidence(
            "System Error",
            "Technical malfunction",
            true,
            date,
            attraction.Id);

        // Use reflection to set the Attraction property since it has a private setter
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        attractionProperty?.SetValue(incidence, attraction);

        // Act
        var dto = incidence.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(incidence.Id, dto.Id);
        Assert.AreEqual("System Error", dto.Title);
        Assert.AreEqual("Technical malfunction", dto.Description);
        Assert.IsTrue(dto.Status);
        Assert.AreEqual(date, dto.Date);
        Assert.AreEqual(attraction.Id, dto.AttractionId);
        Assert.AreEqual("Roller Coaster", dto.AttractionName);
    }

    [TestMethod]
    public void ToDomain_ShouldMapCreateAttractionRequestDtoToAttraction()
    {
        // Arrange
        var dto = new CreateAttractionRequestDto
        {
            Name = "Bumper Cars",
            Description = "Classic bumper cars",
            Type = "Family",
            MinAge = 6,
            Capacity = 40,
        };

        // Act
        var attraction = dto.ToDomain();

        // Assert
        Assert.IsNotNull(attraction);
        Assert.AreEqual("Bumper Cars", attraction.Name);
        Assert.AreEqual("Classic bumper cars", attraction.Description);
        Assert.AreEqual("Family", attraction.Type);
        Assert.AreEqual(6, attraction.MinAge);
        Assert.AreEqual(40, attraction.Capacity);
    }

    [TestMethod]
    public void ToIncidence_ShouldMapCreateIncidenceRequestDtoToIncidence()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var date = new DateTime(2025, 10, 6);
        var dto = new CreateIncidenceRequestDto
        {
            Title = "Maintenance Required",
            Description = "Regular maintenance",
            Status = false,
            Date = date,
        };

        // Act
        var incidence = dto.ToIncidence(attractionId);

        // Assert
        Assert.IsNotNull(incidence);
        Assert.AreEqual("Maintenance Required", incidence.Title);
        Assert.AreEqual("Regular maintenance", incidence.Description);
        Assert.IsFalse(incidence.Status);
        Assert.AreEqual(date, incidence.Date);
        Assert.AreEqual(attractionId, incidence.AttractionId);
    }

    [TestMethod]
    public void ToDto_Collection_ShouldMapListOfAttractionsToListOfDtos()
    {
        // Arrange
        var attractions = new List<Attraction>
        {
            new Attraction("Attraction1", "Desc1", "Extreme", 12, 50),
            new Attraction("Attraction2", "Desc2", "Family", 5, 100),
        };

        // Act
        var dtos = attractions.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Attraction1", dtos[0].Name);
        Assert.AreEqual("Attraction2", dtos[1].Name);
    }

    [TestMethod]
    public void ToListDto_Collection_ShouldMapListOfAttractionsToListOfListDtos()
    {
        // Arrange
        var attractions = new List<Attraction>
        {
            new Attraction("Attraction1", "Desc1", "Extreme", 12, 50),
            new Attraction("Attraction2", "Desc2", "Family", 5, 100),
            new Attraction("Attraction3", "Desc3", "Water", 8, 75),
        };

        // Act
        var dtos = attractions.ToListDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(3, dtos.Count);
        Assert.AreEqual("Attraction1", dtos[0].Name);
        Assert.AreEqual("Attraction2", dtos[1].Name);
        Assert.AreEqual("Attraction3", dtos[2].Name);
    }

    [TestMethod]
    public void ToDto_IncidenceCollection_ShouldMapListOfIncidencesToListOfDtos()
    {
        // Arrange
        var attraction = new Attraction("Test Attraction", "Test Description", "Extreme", 12, 30);
        var incidences = new List<Incidence>
        {
            new Incidence("Issue1", "Desc1", true, DateTime.Now, attraction.Id),
            new Incidence("Issue2", "Desc2", false, DateTime.Now, attraction.Id),
        };

        // Use reflection to set the Attraction property for each incidence
        var attractionProperty = typeof(Incidence).GetProperty("Attraction");
        foreach (var incidence in incidences)
        {
            attractionProperty?.SetValue(incidence, attraction);
        }

        // Act
        var dtos = incidences.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Issue1", dtos[0].Title);
        Assert.AreEqual("Issue2", dtos[1].Title);
        Assert.AreEqual("Test Attraction", dtos[0].AttractionName);
        Assert.AreEqual("Test Attraction", dtos[1].AttractionName);
    }
}
