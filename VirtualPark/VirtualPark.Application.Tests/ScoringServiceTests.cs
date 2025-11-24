// <copyright file="ScoringServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Moq;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ScoringServiceTests
{
    private ScoringService service = null!;

    // Replace concrete repos with mocks
    private Mock<IScoreLogRepository> mockScoreLogRepo = null!;
    private Mock<IVisitorRepository> mockVisitorRepo = null!;
    private Mock<IAttractionRepository> mockAttractionRepo = null!;
    private Mock<IAttractionAccessRepository> mockAccessRepo = null!;
    private Mock<ISpecialEventRepository> mockEventRepo = null!;
    private Mock<IActiveScoringStrategyRepository> mockStrategyRepo = null!;
    private Mock<IScoringStrategyLoader> mockLoader = null!;

    // store created score logs for assertions
    private List<ScoreLog> scoreLogStore = null!;

    [TestInitialize]
    public void Setup()
    {
        // Initialize mocks
        this.mockScoreLogRepo = new Mock<IScoreLogRepository>();
        this.mockVisitorRepo = new Mock<IVisitorRepository>();
        this.mockAttractionRepo = new Mock<IAttractionRepository>();
        this.mockAccessRepo = new Mock<IAttractionAccessRepository>();
        this.mockEventRepo = new Mock<ISpecialEventRepository>();
        this.mockStrategyRepo = new Mock<IActiveScoringStrategyRepository>();

        // capture added score logs
        this.scoreLogStore = new List<ScoreLog>();
        this.mockScoreLogRepo
            .Setup(r => r.Add(It.IsAny<ScoreLog>()))
            .Callback<ScoreLog>(sl => this.scoreLogStore.Add(sl));

        // Setup mock loader with built-in strategies
        this.mockLoader = new Mock<IScoringStrategyLoader>();
        var builtInStrategies = new Dictionary<string, IScoringStrategy>
        {
            ["ScoreByAttractionType"] = new ScoreByAttractionTypeStrategy(),
            ["ScoreByCombo"] = new ScoreByComboStrategy(),
            ["ScoreByEventMultiplier"] = new ScoreByEventMultiplierStrategy(),
        };
        this.mockLoader.Setup(x => x.GetStrategies()).Returns(builtInStrategies);

        // instantiate service with mocks
        this.service = new ScoringService(
            this.mockScoreLogRepo.Object,
            this.mockVisitorRepo.Object,
            this.mockAttractionRepo.Object,
            this.mockAccessRepo.Object,
            this.mockEventRepo.Object,
            this.mockLoader.Object,
            this.mockStrategyRepo.Object);
    }

    [TestMethod]
    public void AwardPoints_WithScoreByAttractionType_CalculatesCorrectPoints()
    {
        // Arrange
        var visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var ticket = new Ticket(DateTime.Now, TicketType.General, Guid.NewGuid()) { VisitorId = visitor.Id };
        var awardedAt = DateTime.Now;
        var attractionAccess = new AttractionAccess(attraction.Id, visitor.Id, ticket.Id, awardedAt, null, EntryMethod.QR);

        // Setup repo returns
        this.mockVisitorRepo.Setup(r => r.GetById(visitor.Id)).Returns(visitor);
        this.mockAttractionRepo.Setup(r => r.GetById(attraction.Id)).Returns(attraction);

        // The access repository provides accesses by visitor and date in the real interface
        this.mockAccessRepo.Setup(r => r.GetAccessesByVisitorAndDate(visitor.Id, awardedAt.Date)).Returns(new[] { attractionAccess });

        // ensure no special event - repository exposes GetAll()
        this.mockEventRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<Domain.SpecialEvent>());

        // Act
        this.service.AwardPoints(visitor.Id, attractionAccess.Id, attraction.Id, awardedAt);

        // Assert - inspect captured score logs
        Assert.AreEqual(1, this.scoreLogStore.Count);
        Assert.AreEqual(100, this.scoreLogStore[0].PointsAwarded);
        Assert.AreEqual("ScoreByAttractionType", this.scoreLogStore[0].StrategyUsed);
        Assert.AreEqual(visitor.Id, this.scoreLogStore[0].VisitorId);
        Assert.AreEqual(attraction.Id, this.scoreLogStore[0].AttractionId);
    }

    [TestMethod]
    public void GetAvailableStrategies_ReturnsAllThreeStrategies()
    {
        var strategies = this.service.GetAvailableStrategies().ToList();

        Assert.AreEqual(3, strategies.Count);
        Assert.IsTrue(strategies.Contains("ScoreByAttractionType"));
        Assert.IsTrue(strategies.Contains("ScoreByCombo"));
        Assert.IsTrue(strategies.Contains("ScoreByEventMultiplier"));
    }

    [TestMethod]
    public void AwardPoints_UsesComboStrategy_CalculatesCorrectPoints()
    {
        // Arrange
        this.service.SetActiveStrategy("ScoreByCombo");

        var visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var ticket = new Ticket(DateTime.Now, TicketType.General, Guid.NewGuid()) { VisitorId = visitor.Id };
        var awardedAt = DateTime.Now;
        var attractionAccess = new AttractionAccess(attraction.Id, visitor.Id, ticket.Id, awardedAt, null, EntryMethod.QR);

        this.mockVisitorRepo.Setup(r => r.GetById(visitor.Id)).Returns(visitor);
        this.mockAttractionRepo.Setup(r => r.GetById(attraction.Id)).Returns(attraction);
        this.mockAccessRepo.Setup(r => r.GetAccessesByVisitorAndDate(visitor.Id, awardedAt.Date)).Returns(new[] { attractionAccess });
        this.mockEventRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<Domain.SpecialEvent>());

        // Act
        this.service.AwardPoints(visitor.Id, attractionAccess.Id, attraction.Id, awardedAt);

        // Assert
        Assert.AreEqual(1, this.scoreLogStore.Count);
        Assert.AreEqual(75, this.scoreLogStore[0].PointsAwarded);
        Assert.AreEqual("ScoreByCombo", this.scoreLogStore[0].StrategyUsed);
    }

    [TestMethod]
    public void GetDailyRanking_ReturnsTop10Ordered()
    {
        // Arrange
        var targetDate = new DateTime(2025, 10, 2);
        var visitor1 = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var visitor2 = new Visitor("Jane", "Smith", "jane@example.com", "password456", new DateTime(1985, 5, 15), MembershipLevel.Premium, Guid.NewGuid());
        var visitor3 = new Visitor("Bob", "Johnson", "bob@example.com", "password789", new DateTime(1992, 3, 20), MembershipLevel.VIP, Guid.NewGuid());

        var attraction = new Attraction("Test Attraction", "Test", "RollerCoaster", 12, 20);

        var scoreLogs = new List<ScoreLog>
        {
            new ScoreLog(visitor1.Id, Guid.NewGuid(), attraction.Id, 100, "ScoreByAttractionType", targetDate),
            new ScoreLog(visitor1.Id, Guid.NewGuid(), attraction.Id, 80, "ScoreByAttractionType", targetDate),
            new ScoreLog(visitor2.Id, Guid.NewGuid(), attraction.Id, 150, "ScoreByCombo", targetDate),
            new ScoreLog(visitor3.Id, Guid.NewGuid(), attraction.Id, 50, "ScoreByAttractionType", targetDate),
        };

        // Setup score log repo to return these logs for the target date
        this.mockScoreLogRepo.Setup(r => r.GetByDate(targetDate)).Returns(scoreLogs.AsQueryable());

        // Setup visitor repo to return visitors when requested (GetById)
        this.mockVisitorRepo.Setup(r => r.GetById(visitor1.Id)).Returns(visitor1);
        this.mockVisitorRepo.Setup(r => r.GetById(visitor2.Id)).Returns(visitor2);
        this.mockVisitorRepo.Setup(r => r.GetById(visitor3.Id)).Returns(visitor3);

        // Act
        var ranking = this.service.GetDailyRanking(targetDate).ToList();

        // Assert
        Assert.AreEqual(3, ranking.Count);
        Assert.AreEqual(1, ranking[0].Position);
        Assert.AreEqual(visitor1.Id, ranking[0].VisitorId);
        Assert.AreEqual("John Doe", ranking[0].VisitorName);
        Assert.AreEqual(180, ranking[0].TotalPoints);
        Assert.AreEqual(2, ranking[1].Position);
        Assert.AreEqual(visitor2.Id, ranking[1].VisitorId);
        Assert.AreEqual(150, ranking[1].TotalPoints);
        Assert.AreEqual(3, ranking[2].Position);
        Assert.AreEqual(visitor3.Id, ranking[2].VisitorId);
        Assert.AreEqual(50, ranking[2].TotalPoints);
    }

    [TestMethod]
    public void GetDailyRanking_WithTop5Parameter_ReturnsOnly5()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLogs = new List<ScoreLog>();
        var visitors = new List<Visitor>();

        var attraction = new Attraction("Test Attraction", "Test", "RollerCoaster", 12, 20);

        for (var i = 0; i < 15; i++)
        {
            var visitor = new Visitor($"User{i}", "Test", $"user{i}@example.com", "password", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
            visitors.Add(visitor);
        }

        for (var i = 0; i < 15; i++)
        {
            scoreLogs.Add(new ScoreLog(visitors[i].Id, Guid.NewGuid(), attraction.Id, 100 + i, "ScoreByAttractionType", targetDate));
            this.mockVisitorRepo.Setup(r => r.GetById(visitors[i].Id)).Returns(visitors[i]);
        }

        this.mockScoreLogRepo.Setup(r => r.GetByDate(targetDate)).Returns(scoreLogs.AsQueryable());

        var ranking = this.service.GetDailyRanking(targetDate, 5).ToList();

        Assert.AreEqual(5, ranking.Count);
    }

    [TestMethod]
    public void GetDailyRanking_NoScoreLogs_ReturnsEmpty()
    {
        var targetDate = new DateTime(2025, 10, 2);

        this.mockScoreLogRepo.Setup(r => r.GetByDate(targetDate)).Returns(Enumerable.Empty<ScoreLog>().AsQueryable());

        var ranking = this.service.GetDailyRanking(targetDate).ToList();

        Assert.AreEqual(0, ranking.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void AwardPoints_ThrowsWhenVisitorNotFound()
    {
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var awardedAt = DateTime.Now;

        // visitor repo returns null -> simulate not found
        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns((Visitor?)null);

        this.service.AwardPoints(visitorId, attractionAccessId, attractionId, awardedAt);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void AwardPoints_ThrowsWhenAttractionNotFound()
    {
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var awardedAt = DateTime.Now;
        var visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());

        this.mockVisitorRepo.Setup(r => r.GetById(visitor.Id)).Returns(visitor);

        // attraction repo returns null
        this.mockAttractionRepo.Setup(r => r.GetById(attractionId)).Returns((Attraction?)null);

        this.service.AwardPoints(visitor.Id, attractionAccessId, attractionId, awardedAt);
    }

    [TestMethod]
    public void GetActiveStrategy_ReturnsDefault_WhenNoActiveStrategySet()
    {
        // Act
        var strategy = this.service.GetActiveStrategy();

        // Assert
        Assert.AreEqual("ScoreByAttractionType", strategy);
    }

    [TestMethod]
    public void GetActiveStrategy_ReturnsSetStrategy_WhenStrategyIsSet()
    {
        // Arrange
        this.service.SetActiveStrategy("ScoreByCombo");

        // Act
        var strategy = this.service.GetActiveStrategy();

        // Assert
        Assert.AreEqual("ScoreByCombo", strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SetActiveStrategy_ThrowsException_WhenStrategyNotFound()
    {
        // Act
        this.service.SetActiveStrategy("NonExistentStrategy");
    }

    [TestMethod]
    public void GetDailyRanking_SkipsVisitorsNotFound()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var visitorId = Guid.NewGuid(); // Visitor that doesn't exist

        var attraction = new Attraction("Test Attraction", "Test", "RollerCoaster", 12, 20);

        var scoreLog = new ScoreLog(visitorId, Guid.NewGuid(), attraction.Id, 100, "ScoreByAttractionType", targetDate);

        this.mockScoreLogRepo.Setup(r => r.GetByDate(targetDate)).Returns(new[] { scoreLog }.AsQueryable());

        // visitor repo returns null for that visitor
        this.mockVisitorRepo.Setup(r => r.GetById(visitorId)).Returns((Visitor?)null);

        var ranking = this.service.GetDailyRanking(targetDate).ToList();

        Assert.AreEqual(0, ranking.Count);
    }

    [TestMethod]
    public void AwardPoints_WithEventMultiplierStrategy_CalculatesCorrectPoints()
    {
        // Arrange
        this.service.SetActiveStrategy("ScoreByEventMultiplier");

        var visitor = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var attraction = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        var specialEvent = new Domain.SpecialEvent("Special Day", DateTime.Today.AddDays(1), 100, 0m);
        var ticket = new Ticket(DateTime.Today, TicketType.General, Guid.NewGuid()) { VisitorId = visitor.Id };
        var awardedAt = DateTime.Today;
        var attractionAccess = new AttractionAccess(attraction.Id, visitor.Id, ticket.Id, awardedAt, null, EntryMethod.QR);

        this.mockVisitorRepo.Setup(r => r.GetById(visitor.Id)).Returns(visitor);
        this.mockAttractionRepo.Setup(r => r.GetById(attraction.Id)).Returns(attraction);
        this.mockAccessRepo.Setup(r => r.GetAccessesByVisitorAndDate(visitor.Id, awardedAt.Date)).Returns(new[] { attractionAccess });
        this.mockEventRepo.Setup(r => r.GetAll()).Returns(new[] { specialEvent });

        // Act
        this.service.AwardPoints(visitor.Id, attractionAccess.Id, attraction.Id, awardedAt);

        // Assert
        Assert.AreEqual(1, this.scoreLogStore.Count);
        Assert.AreEqual("ScoreByEventMultiplier", this.scoreLogStore[0].StrategyUsed);
    }
}
