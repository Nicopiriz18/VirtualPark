// <copyright file="AttractionControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Attractions;
using VirtualPark.Domain;
using VirtualPark.DTOs.Attractions.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;
[TestClass]
public class AttractionControllerTests
{
    [TestMethod]
    public void GetAll_ReturnsOk_WithListOfAttractions()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var mockService = new Mock<IAttractionService>();
        mockService.Setup(s => s.GetAll()).Returns([attraction]);
        var controller = new AttractionsController(mockService.Object);

        var result = controller.Get() as OkObjectResult;

        Assert.IsNotNull(result);
        var value = result.Value as IEnumerable<AttractionListDto>;
        if (value != null)
        {
            Assert.AreEqual(1, value.Count());
            var firstAttraction = value.First();
            Assert.AreEqual("Roller Coaster", firstAttraction.Name);
            Assert.AreEqual("Montaña Rusa", firstAttraction.Type);
        }
    }

    [TestMethod]
    public void GetById_ReturnsOk_WhenAttractionExists()
    {
        var id = Guid.NewGuid();
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30) { Id = id };
        var mockService = new Mock<IAttractionService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetById(id)).Returns(attraction);
        var controller = new AttractionsController(mockService.Object);
        var result = controller.Get(id) as OkObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var attractionDto = result.Value as AttractionDetailDto;
        Assert.IsNotNull(attractionDto);
        Assert.AreEqual(id, attractionDto.Id);
        Assert.AreEqual("Roller Coaster", attractionDto.Name);
        Assert.AreEqual("Montaña Rusa", attractionDto.Type);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetById_ReturnsNotFound_WhenNotExists()
    {
        var mockService = new Mock<IAttractionService>();
        mockService.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Attraction?)null);
        var controller = new AttractionsController(mockService.Object);

        var result = controller.Get(Guid.NewGuid());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void Post_ReturnsCreatedAtAction_WithAttraction()
    {
        var attraction = new Attraction("Ferris Wheel", "Slow ride", "Rueda", 0, 40);
        var mockService = new Mock<IAttractionService>();
        mockService.Setup(s => s.Create(It.IsAny<Attraction>())).Returns(attraction);
        var controller = new AttractionsController(mockService.Object);

        var request = new VirtualPark.DTOs.Attractions.Requests.CreateAttractionRequestDto
        {
            Name = "Ferris Wheel",
            Description = "Slow ride",
            Type = "Rueda",
            MinAge = 0,
            Capacity = 40,
        };

        var result = controller.Post(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Get", result.ActionName);
        var attractionDto = result.Value as AttractionDto;
        Assert.IsNotNull(attractionDto);
        Assert.AreEqual("Ferris Wheel", attractionDto.Name);
        Assert.AreEqual("Rueda", attractionDto.Type);
    }

    [TestMethod]
    public void Put_ReturnsNoContent()
    {
        var mockService = new Mock<IAttractionService>();
        var controller = new AttractionsController(mockService.Object);

        var request = new VirtualPark.DTOs.Attractions.Requests.UpdateAttractionRequestDto
        {
            Name = "Haunted House",
            Description = "Scary",
            Type = "Casa",
            MinAge = 15,
            Capacity = 20,
        };

        var result = controller.Put(Guid.NewGuid(), request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void Delete_ReturnsNoContent()
    {
        var mockService = new Mock<IAttractionService>();
        var controller = new AttractionsController(mockService.Object);

        var result = controller.Delete(Guid.NewGuid());

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
