// <copyright file="AttractionServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Attractions;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class AttractionServiceTests
{
    private readonly Guid id = Guid.NewGuid();

    [TestMethod]
    public void GetAll_ReturnsList()
    {
        var data = new List<Attraction> { new Attraction("Roller Coaster", "Fast", "Montaña Rusa", 12, 30) };
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.GetAll()).Returns(data);
        var service = new AttractionService(mockRepo.Object);

        var result = service.GetAll();

        Assert.AreEqual(1, result.Count());
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void GetById_ReturnsAttraction_WhenExists()
    {
        var attraction = new Attraction("Ferris Wheel", "Panoramic", "Rueda", 0, 40) { Id = this.id };
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.GetById(this.id)).Returns(attraction);
        var service = new AttractionService(mockRepo.Object);

        var result = service.GetById(this.id);

        Assert.IsNotNull(result);
        Assert.AreEqual(this.id, result.Id);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void Create_SavesAndReturnsAttraction()
    {
        var toCreate = new Attraction("Haunted House", "Scary", "Casa", 15, 20);
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.Add(toCreate)).Returns(toCreate);
        var service = new AttractionService(mockRepo.Object);

        var result = service.Create(toCreate);

        Assert.AreSame(toCreate, result);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void Update_CallsRepositoryUpdate()
    {
        var existingAttraction = new Attraction("Old Name", "Old Desc", "Type", 10, 25);
        var updatedAttraction = new Attraction("Simulador 3D", "VR", "Simulador", 10, 25);
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.GetById(this.id)).Returns(existingAttraction);
        mockRepo.Setup(r => r.Update(this.id, existingAttraction));
        var service = new AttractionService(mockRepo.Object);

        service.Update(this.id, updatedAttraction);

        mockRepo.Verify(r => r.GetById(this.id), Times.Once);
        mockRepo.Verify(r => r.Update(this.id, existingAttraction), Times.Once);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_Throws_WhenAttractionNotFound()
    {
        var updatedAttraction = new Attraction("New Name", "New Desc", "Type", 10, 25);
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.GetById(this.id)).Returns((Attraction)null!);
        var service = new AttractionService(mockRepo.Object);

        service.Update(this.id, updatedAttraction);
    }

    [TestMethod]
    public void Delete_CallsRepositoryDelete()
    {
        var mockRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.Delete(this.id));
        var service = new AttractionService(mockRepo.Object);

        service.Delete(this.id);

        mockRepo.Verify(r => r.Delete(this.id), Times.Once);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void AddIncidence_AddsNewIncidence()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30) { Id = this.id };
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.Add(It.IsAny<Incidence>())).Returns((Incidence i) => i);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        var result = service.AddIncidence(this.id, "Broken", "Engine failure", true, DateTime.Now);

        Assert.IsNotNull(result);
        Assert.AreEqual("Broken", result.Title);
        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    public void RemoveIncidence_RemovesCorrectIncidence()
    {
        var attraction = new Attraction("Ferris Wheel", "Panoramic", "Rueda", 0, 40) { Id = this.id };
        var incidence = new Incidence("Test", "desc", true, DateTime.Now, this.id);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);
        mockIncidenceRepo.Setup(r => r.Delete(incidence.Id));

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.RemoveIncidence(this.id, incidence.Id);

        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    public void CloseIncidence_ClosesTheIncidence()
    {
        var attraction = new Attraction("Haunted House", "Scary", "Casa", 15, 20) { Id = this.id };
        var incidence = new Incidence("Issue1", "desc1", true, DateTime.Now, this.id);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);
        mockIncidenceRepo.Setup(r => r.Update(incidence));

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.CloseIncidence(this.id, incidence.Id);

        Assert.IsFalse(incidence.Status);
        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    public void ReopenIncidence_ReopensTheIncidence()
    {
        var attraction = new Attraction("Haunted House", "Scary", "Casa", 15, 20) { Id = this.id };
        var incidence = new Incidence("Issue1", "desc1", false, DateTime.Now, this.id);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);
        mockIncidenceRepo.Setup(r => r.Update(incidence));

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.ReopenIncidence(this.id, incidence.Id);

        Assert.IsTrue(incidence.Status);
        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    public void GetIncidences_ReturnsAllIncidences()
    {
        var attraction = new Attraction("Haunted House", "Scary", "Casa", 15, 20) { Id = this.id };
        var incidence1 = new Incidence("Issue1", "desc1", true, DateTime.Now, this.id);
        var incidence2 = new Incidence("Issue2", "desc2", false, DateTime.Now, this.id);
        var incidences = new List<Incidence> { incidence1, incidence2 };

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetByAttractionId(this.id)).Returns(incidences);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        var result = service.GetIncidences(this.id);

        Assert.AreEqual(2, result.Count());
        CollectionAssert.Contains(result.ToList(), incidence1);
        CollectionAssert.Contains(result.ToList(), incidence2);
        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void AddIncidence_Throws_WhenAttractionNotFound()
    {
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.AddIncidence(Guid.NewGuid(), "Title", "Desc", true, DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void RemoveIncidence_Throws_WhenAttractionNotFound()
    {
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.RemoveIncidence(Guid.NewGuid(), Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetIncidences_Throws_WhenAttractionNotFound()
    {
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.GetIncidences(Guid.NewGuid());
    }

    [TestMethod]
    public void GetActiveIncidences_ReturnsOnlyOpenIncidences()
    {
        var attraction = new Attraction("Simulador 3D", "VR", "Simulador", 12, 25) { Id = this.id };
        var openIncidence = new Incidence("Open Issue", "desc1", true, DateTime.Now, this.id);
        var closedIncidence = new Incidence("Closed Issue", "desc2", false, DateTime.Now, this.id);
        var incidences = new List<Incidence> { openIncidence, closedIncidence };

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(this.id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetByAttractionId(this.id)).Returns(incidences);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        var result = service.GetActiveIncidences(this.id);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(openIncidence.Id, result.First().Id);
        mockAttractionRepo.VerifyAll();
        mockIncidenceRepo.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void CloseIncidence_Throws_WhenAttractionNotFound()
    {
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.CloseIncidence(Guid.NewGuid(), Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void CloseIncidence_Throws_WhenIncidenceNotFound()
    {
        var attraction = new Attraction("Haunted", "Scary", "Casa", 10, 20) { Id = Guid.NewGuid() };

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Incidence)null!);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.CloseIncidence(attraction.Id, Guid.NewGuid()); // incidencia inexistente
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ReopenIncidence_Throws_WhenAttractionNotFound()
    {
        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.ReopenIncidence(Guid.NewGuid(), Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ReopenIncidence_Throws_WhenIncidenceNotFound()
    {
        var attraction = new Attraction("Ferris", "View", "Rueda", 0, 30) { Id = Guid.NewGuid() };

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Incidence)null!);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.ReopenIncidence(attraction.Id, Guid.NewGuid()); // incidencia inexistente
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RemoveIncidence_Throws_WhenIncidenceBelongsToOtherAttraction()
    {
        var attractionId = Guid.NewGuid();
        var otherAttractionId = Guid.NewGuid();
        var attraction = new Attraction("Roller Coaster", "Fast", "Montaña Rusa", 12, 30) { Id = attractionId };
        var incidence = new Incidence("Issue", "desc", true, DateTime.Now, otherAttractionId);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.RemoveIncidence(attractionId, incidence.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CloseIncidence_Throws_WhenIncidenceBelongsToOtherAttraction()
    {
        var attractionId = Guid.NewGuid();
        var otherAttractionId = Guid.NewGuid();
        var attraction = new Attraction("Ferris Wheel", "Panoramic", "Rueda", 0, 40) { Id = attractionId };
        var incidence = new Incidence("Issue", "desc", true, DateTime.Now, otherAttractionId);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.CloseIncidence(attractionId, incidence.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ReopenIncidence_Throws_WhenIncidenceBelongsToOtherAttraction()
    {
        var attractionId = Guid.NewGuid();
        var otherAttractionId = Guid.NewGuid();
        var attraction = new Attraction("Haunted House", "Scary", "Casa", 15, 20) { Id = attractionId };
        var incidence = new Incidence("Issue", "desc", false, DateTime.Now, otherAttractionId);

        var mockAttractionRepo = new Mock<IAttractionRepository>(MockBehavior.Strict);
        mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockIncidenceRepo = new Mock<IIncidenceRepository>(MockBehavior.Strict);
        mockIncidenceRepo.Setup(r => r.GetById(incidence.Id)).Returns(incidence);

        var service = new AttractionIncidenceService(mockAttractionRepo.Object, mockIncidenceRepo.Object);

        service.ReopenIncidence(attractionId, incidence.Id);
    }
}
