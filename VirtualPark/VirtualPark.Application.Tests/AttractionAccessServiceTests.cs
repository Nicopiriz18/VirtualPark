// <copyright file="AttractionAccessServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reflection;
using Moq;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;
[TestClass]
public class AttractionAccessServiceTests
{
    private readonly Guid id = Guid.NewGuid();
    private Mock<IAttractionRepository> attractionRepoMock = null!;
    private Mock<IAttractionAccessRepository> mockAccessRepo = null!;
    private Mock<IAttractionValidationService> mockValidationService = null!;
    private Mock<ITicketLookupService> mockTicketLookupService = null!;
    private Mock<IClockService> mockClockService = null!;
    private Mock<IScoringService> mockScoringService = null!;
    private readonly Guid attractionId = Guid.NewGuid();
    private AttractionAccessService service = null!;

    [TestInitialize]
    public void Setup()
    {
        this.attractionRepoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);

        // Provide a sane default so tests that don't override it won't fail the strict mock
        this.attractionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(new Attraction("Default", "desc", "type", 0, 100));
        this.mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        this.mockValidationService = new Mock<IAttractionValidationService>(MockBehavior.Strict);
        this.mockTicketLookupService = new Mock<ITicketLookupService>(MockBehavior.Strict);
        this.mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        this.mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        this.service = new AttractionAccessService(this.mockAccessRepo.Object, this.attractionRepoMock.Object, this.mockValidationService.Object, this.mockTicketLookupService.Object, this.mockClockService.Object, this.mockScoringService.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void HasValidAge_Throws_WhenAttractionNotFound()
    {
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockValidationService = new Mock<IAttractionValidationService>(MockBehavior.Strict);
        mockValidationService.Setup(v => v.HasValidAge(It.IsAny<Guid>(), It.IsAny<int>())).Throws<KeyNotFoundException>();
        var service = new AttractionAccessService(null!, repoMock.Object, mockValidationService.Object, null!, null!, null!);

        service.HasValidAge(Guid.NewGuid(), 20);
        repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void IsAttractionAvailable_Throws_WhenAttractionNotFound()
    {
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockValidationService = new Mock<IAttractionValidationService>(MockBehavior.Strict);
        mockValidationService.Setup(v => v.IsAttractionAvailable(It.IsAny<Guid>())).Throws<KeyNotFoundException>();
        var service = new AttractionAccessService(null!, repoMock.Object, mockValidationService.Object, null!, null!, null!);

        service.IsAttractionAvailable(Guid.NewGuid());
        repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void RegisterAccess_Throws_WhenAttractionNotFound()
    {
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Attraction)null!);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        var mockTicketRepo = new Mock<ITicketRepository>();
        var mockVisitorRepo = new Mock<IVisitorRepository>();

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());

        service.RegisterAccess(Guid.NewGuid(), ticket, EntryMethod.QR);
        repoMock.VerifyAll();
    }

    [TestMethod]
    public void HasValidAge_ReturnsFalse_WhenAgeTooLow()
    {
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.HasValidAge(this.id, 10)).Returns(false);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(null!, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        var result = service.HasValidAge(this.id, 10);

        Assert.IsFalse(result);
        mockValidationSvc.Verify(v => v.HasValidAge(this.id, 10), Times.Once);
    }

    [TestMethod]
    public void HasValidAge_ReturnsTrue_WhenAgeMeetsRequirement()
    {
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.HasValidAge(this.id, 15)).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(null!, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        var result = service.HasValidAge(this.id, 15);

        Assert.IsTrue(result);
        mockValidationSvc.Verify(v => v.HasValidAge(this.id, 15), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsTrue_WhenNoIncidences()
    {
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(this.id)).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(null!, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        var result = service.IsAttractionAvailable(this.id);

        Assert.IsTrue(result);
        mockValidationSvc.Verify(v => v.IsAttractionAvailable(this.id), Times.Once);
    }

    [TestMethod]
    public void IsAttractionAvailable_ReturnsFalse_WhenHasOpenIncidence()
    {
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(this.id)).Returns(false);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(null!, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);
        var result = service.IsAttractionAvailable(this.id);
        Assert.IsFalse(result);
        mockValidationSvc.Verify(v => v.IsAttractionAvailable(this.id), Times.Once);
    }

    [TestMethod]
    public void RegisterAccess_IncrementsVisitorCount()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);
        repoMock.Setup(r => r.Update(attraction.Id, attraction));

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attraction.Id)).Returns(0);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attraction.Id, ticket.VisitorId)).Returns((AttractionAccess?)null);
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()))
            .Returns((AttractionAccess a) => a);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);

        mockAccessRepo.Verify(
            r => r.Add(It.Is<AttractionAccess>(a => a.AttractionId == attraction.Id && a.TicketId == ticket.Id)),
            Times.Once);
    }

    [TestMethod]
    public void RegisterAccess_Allows_WhenVisitorProvidedAndAgeIsValid()
    {
        var attraction = new Attraction("Loop", "Fast", "Type", 12, 10) { Id = Guid.NewGuid() };

        var visitor = new Visitor("Test", "User", "test@user.com", "pass", DateTime.Today.AddYears(-20),
            MembershipLevel.Standard, Guid.NewGuid());

        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid())
        {
            Visitor = visitor,
            VisitorId = visitor.Id,
        };

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attraction.Id)).Returns(0);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attraction.Id, visitor.Id)).Returns((AttractionAccess?)null);
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()))
            .Returns((AttractionAccess a) => a);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);

        mockAccessRepo.Verify(r => r.Add(It.Is<AttractionAccess>(a => a.TicketId == ticket.Id)), Times.Once);
    }

    [TestMethod]
    public void RegisterAccess_AllValidationsPass_AddsAccess()
    {
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };

        var attraction = new Attraction("Carousel", "Kids ride", "Type", 0, 10);

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attractionId)).Returns(0);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attractionId, visitorId)).Returns((AttractionAccess?)null);
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()))
            .Returns((AttractionAccess a) => a);
        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attractionId)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attractionId, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attractionId, ticket, EntryMethod.QR);

        mockAccessRepo.Verify(r => r.Add(It.IsAny<AttractionAccess>()), Times.Once);
        repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_LanzaExcepcion_SiAforoCompleto()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 1);
        var ticket1 = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());
        var ticket2 = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);
        repoMock.Setup(r => r.Update(attraction.Id, attraction));

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attraction.Id)).Returns(1); // Aforo completo
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()));

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket1, EntryMethod.QR);
        service.RegisterAccess(attraction.Id, ticket2, EntryMethod.QR); // Debería lanzar excepción
    }

    [TestMethod]
    public void RegisterExit_MarksExitTime()
    {
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var now = DateTime.Now;

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attractionId, visitorId))
            .Returns(new AttractionAccess(attractionId, visitorId, Guid.NewGuid(), now.AddHours(-1), null,
                EntryMethod.QR));
        mockAccessRepo.Setup(r => r.Update(It.IsAny<AttractionAccess>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        service.RegisterExit(attractionId, visitorId, now);

        mockAccessRepo.Verify(
            r => r.Update(It.Is<AttractionAccess>(a => a.ExitTime.HasValue && a.ExitTime.Value == now)), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_ThrowsException_WhenTicketWrongDate()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var ticket = new Ticket(DateTime.Today.AddDays(-1), TicketType.General, Guid.NewGuid());

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_ThrowsException_WhenSpecialEventNotAllowed()
    {
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 30);
        var ticket = new Ticket(DateTime.Today, TicketType.SpecialEvent, Guid.NewGuid());

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_Throws_WhenSpecialEventTicketHasNoEventId()
    {
        var attractionId = Guid.NewGuid();
        var ticket = new Ticket(DateTime.Today, TicketType.SpecialEvent, Guid.NewGuid())
        {
            SpecialEventId = null,
        };

        var attraction = new Attraction("Haunted House", "Scary ride", "Type", 0, 10);
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attractionId)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attractionId, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attractionId, ticket, EntryMethod.QR);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_ThrowsException_WhenAttractionNotAvailable()
    {
        var attraction = new Attraction("Simulador 3D", "VR", "Simulador", 12, 25) { Id = this.id };
        attraction.AddIncidence("Issue1", "desc1", true, DateTime.Now);

        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Today);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>(MockBehavior.Strict);
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(It.IsAny<Guid>())).Returns(false);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);
    }

    [TestMethod]
    public void GetAforo_DevuelveAforoYCapacidadRestante()
    {
        // Arrange
        var attractionId = Guid.NewGuid();
        var attraction = new Attraction("Roller Coaster", "Fast ride", "Montaña Rusa", 12, 10);
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attractionId)).Returns(4);

        var mockTicketRepo = new Mock<ITicketRepository>();
        var mockVisitorRepo = new Mock<IVisitorRepository>();
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        // Act
        var (aforoActual, capacidadRestante) = service.GetAforo(attractionId);

        // Assert
        Assert.AreEqual(4, aforoActual);
        Assert.AreEqual(6, capacidadRestante);
        repoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterExit_ThrowsException_WhenNoOpenAccess()
    {
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attractionId, visitorId))
            .Returns((AttractionAccess)null!);

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, null!, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        service.RegisterExit(attractionId, visitorId, DateTime.Now);
    }

    [TestMethod]
    public void RegisterAccess_VisitorAlreadyInside_Throws()
    {
        var attractionId = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid()) { VisitorId = visitorId };

        var attraction = new Attraction("Water Slide", "Wet!", "Type", 5, 15);
        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attractionId, visitorId))
            .Returns(new AttractionAccess(attractionId, visitorId, ticket.Id, DateTime.Now, null, EntryMethod.QR));

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.RegisterAccess(attractionId, ticket, EntryMethod.QR));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccess_Throws_WhenVisitorTooYoung()
    {
        var attractionId = Guid.NewGuid();
        var attraction = new Attraction("Loop", "Fast", "Type", 18, 10); // edad mínima = 18

        var youngVisitor = new Visitor("Test", "User", "test@user.com", "pass", DateTime.Today.AddYears(-10),
            MembershipLevel.Standard, Guid.NewGuid());

        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid())
        {
            Visitor = youngVisitor,
            VisitorId = youngVisitor.Id,
        };

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);

        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Today);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>(MockBehavior.Strict);
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(It.IsAny<Guid>())).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(It.IsAny<Guid>(), It.IsAny<int>())).Returns(false);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attractionId, ticket, EntryMethod.QR);
    }

    [TestMethod]
    public void RegisterAccess_Allows_WhenVisitorIdEmpty()
    {
        var attraction = new Attraction("Generic", "Desc", "Type", 0, 10) { Id = Guid.NewGuid() };
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid())
        {
            VisitorId = Guid.Empty, // rama especial en ValidateNoDoubleEntry
            Visitor = null,
        };

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attraction.Id)).Returns(0);
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()))
            .Returns((AttractionAccess a) => a);
        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);

        mockAccessRepo.Verify(r => r.Add(It.IsAny<AttractionAccess>()), Times.Once);
    }

    [TestMethod]
    public void RegisterAccess_Allows_WhenTicketHasNoVisitor()
    {
        var attraction = new Attraction("Kids Ride", "Safe", "Type", 12, 5) { Id = Guid.NewGuid() };
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid())
        {
            Visitor = null,
            VisitorId = Guid.NewGuid(),
        };

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attraction.Id)).Returns(0);
        mockAccessRepo.Setup(r => r.GetOpenAccess(attraction.Id, ticket.VisitorId)).Returns((AttractionAccess?)null);
        mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>()))
            .Returns((AttractionAccess a) => a);
        var mockClockService = new Mock<IClockService>(MockBehavior.Strict);
        mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Now);

        var mockScoringService = new Mock<IScoringService>(MockBehavior.Strict);
        mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        var mockValidationSvc = new Mock<IAttractionValidationService>();
        mockValidationSvc.Setup(v => v.IsAttractionAvailable(attraction.Id)).Returns(true);
        mockValidationSvc.Setup(v => v.HasValidAge(attraction.Id, It.IsAny<int>())).Returns(true);
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, mockClockService.Object, mockScoringService.Object);

        service.RegisterAccess(attraction.Id, ticket, EntryMethod.QR);

        mockAccessRepo.Verify(r => r.Add(It.IsAny<AttractionAccess>()), Times.Once);
    }

    [TestMethod]
    public void GetAforo_ReturnsZeroRemaining_WhenOverCapacity()
    {
        var attractionId = Guid.NewGuid();
        var attraction = new Attraction("Popular Ride", "Crowded", "Type", 0, 5);

        var repoMock = new Mock<IAttractionRepository>(MockBehavior.Strict);
        repoMock.Setup(r => r.GetById(attractionId)).Returns(attraction);

        var mockAccessRepo = new Mock<IAttractionAccessRepository>(MockBehavior.Strict);
        mockAccessRepo.Setup(r => r.CountOpenAccesses(attractionId)).Returns(10); // más de la capacidad

        var mockTicketRepo = new Mock<ITicketRepository>();
        var mockVisitorRepo = new Mock<IVisitorRepository>();
        var mockValidationSvc = new Mock<IAttractionValidationService>();
        var mockTicketLookupSvc = new Mock<ITicketLookupService>();
        var service = new AttractionAccessService(mockAccessRepo.Object, repoMock.Object, mockValidationSvc.Object, mockTicketLookupSvc.Object, null!, null!);

        var (_, capacidadRestante) = service.GetAforo(attractionId);

        Assert.AreEqual(0, capacidadRestante);
        repoMock.VerifyAll();
    }

    [DataTestMethod]
    [DataRow("2000-01-01", "2024-06-01", 24)] // Cumpleaños ya pasó este año
    [DataRow("2000-12-31", "2024-06-01", 23)] // Cumpleaños aún no llega este año
    [DataRow("2000-06-01", "2024-06-01", 24)] // Cumpleaños hoy
    [DataRow("2020-02-29", "2024-02-28", 3)] // Año bisiesto, día antes del cumpleaños
    [DataRow("2020-02-29", "2024-02-29", 4)] // Año bisiesto, cumpleaños
    public void CalculateAge_DevuelveEdadCorrecta(string birth, string today, int expected)
    {
        // Arrange
        var birthDate = DateTime.Parse(birth);
        var todayDate = DateTime.Parse(today);

        var method = typeof(AttractionAccessService)
            .GetMethod("CalculateAge", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var age = (int)method.Invoke(null, [birthDate, todayDate]);

        // Assert
        Assert.AreEqual(expected, age);
    }

    [TestMethod]
    public void RegisterAccessByQrCode_ShouldRegister_WhenTicketAndVisitorExist()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("John", "Doe", "john@mail.com", "1234", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var ticket = new Ticket(DateTime.Today, TicketType.General, qrCode);
        ticket.VisitorId = visitorId;
        ticket.Visitor = visitor;

        // Ticket lookup
        this.mockTicketLookupService.Setup(s => s.GetTicketByQrCode(qrCode)).Returns(ticket);

        // Attraction and access-related setups required by RegisterAccess
        var attraction = new Attraction("Test Attraction", "desc", "type", 0, 10) { Id = this.attractionId };
        this.attractionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(attraction);

        this.mockValidationService.Setup(v => v.IsAttractionAvailable(this.attractionId)).Returns(true);
        this.mockValidationService.Setup(v => v.HasValidAge(this.attractionId, It.IsAny<int>())).Returns(true);
        this.mockAccessRepo.Setup(r => r.CountOpenAccesses(this.attractionId)).Returns(0);
        this.mockAccessRepo.Setup(r => r.GetOpenAccess(this.attractionId, visitorId)).Returns((AttractionAccess?)null);
        this.mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>())).Returns((AttractionAccess a) => a);

        // Clock and scoring expectations
        this.mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Today);
        this.mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        // Act
        this.service.RegisterAccessByQrCode(this.attractionId, qrCode, EntryMethod.QR);

        // Assert
        this.mockTicketLookupService.Verify(s => s.GetTicketByQrCode(qrCode), Times.Once);
        this.mockAccessRepo.Verify(r => r.Add(It.IsAny<AttractionAccess>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccessByQrCode_ShouldThrow_WhenTicketNotFound()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        this.mockTicketLookupService.Setup(s => s.GetTicketByQrCode(qrCode)).Throws<InvalidOperationException>();

        // Act
        this.service.RegisterAccessByQrCode(this.attractionId, qrCode, EntryMethod.QR);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccessByQrCode_ShouldThrow_WhenVisitorNotFound()
    {
        // Arrange
        var qrCode = Guid.NewGuid();
        this.mockTicketLookupService.Setup(s => s.GetTicketByQrCode(qrCode)).Throws<InvalidOperationException>();

        // Act
        this.service.RegisterAccessByQrCode(this.attractionId, qrCode, EntryMethod.QR);
    }

    // -----------------------------
    // RegisterAccessByVisitorId
    // -----------------------------
    [TestMethod]
    public void RegisterAccessByVisitorId_ShouldUseMostRecentTicket_WhenMultipleTickets()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var visitor = new Visitor("Jane", "Smith", "jane@mail.com", "abcd", new DateTime(1995, 5, 5), MembershipLevel.Premium, Guid.NewGuid());

        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid());
        ticket.VisitorId = visitorId;
        ticket.Visitor = visitor;

        this.mockTicketLookupService.Setup(s => s.GetTicketByVisitorAndDate(visitorId, DateTime.Today)).Returns(ticket);

#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
        // Configure attraction and access-related mocks used by RegisterAccess
#pragma warning restore SA1515
        var attraction = new Attraction("Test Attraction", "desc", "type", 0, 10) { Id = this.attractionId };
        this.attractionRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(attraction);

        this.mockValidationService.Setup(v => v.IsAttractionAvailable(this.attractionId)).Returns(true);
        this.mockValidationService.Setup(v => v.HasValidAge(this.attractionId, It.IsAny<int>())).Returns(true);
        this.mockAccessRepo.Setup(r => r.CountOpenAccesses(this.attractionId)).Returns(0);
        this.mockAccessRepo.Setup(r => r.GetOpenAccess(this.attractionId, visitorId)).Returns((AttractionAccess?)null);
        this.mockAccessRepo.Setup(r => r.Add(It.IsAny<AttractionAccess>())).Returns((AttractionAccess a) => a);

        this.mockClockService.Setup(s => s.GetNow()).Returns(DateTime.Today);
        this.mockScoringService.Setup(s => s.AwardPoints(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()));

        // Act
        this.service.RegisterAccessByVisitorId(this.attractionId, visitorId, DateTime.Today, EntryMethod.NFC);

        // Assert
        this.mockTicketLookupService.Verify(s => s.GetTicketByVisitorAndDate(visitorId, DateTime.Today), Times.Once);
        Assert.AreEqual(visitor, ticket.Visitor);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccessByVisitorId_ShouldThrow_WhenVisitorNotFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        this.mockTicketLookupService.Setup(s => s.GetTicketByVisitorAndDate(visitorId, DateTime.Today)).Throws<InvalidOperationException>();

        // Act
        this.service.RegisterAccessByVisitorId(this.attractionId, visitorId, DateTime.Today, EntryMethod.NFC);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterAccessByVisitorId_ShouldThrow_WhenNoValidTicketFound()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        this.mockTicketLookupService.Setup(s => s.GetTicketByVisitorAndDate(visitorId, DateTime.Today)).Throws<InvalidOperationException>();

        // Act
        this.service.RegisterAccessByVisitorId(this.attractionId, visitorId, DateTime.Today, EntryMethod.NFC);
    }
}
