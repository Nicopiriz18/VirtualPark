// <copyright file="ScoreLogRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;
using VirtualPark.Infrastructure.Repositories.ScoreLogs;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class ScoreLogRepositoryTests
{
    private ParkDbContext context = null!;
    private ScoreLogRepository repository = null!;
    private Visitor visitor1 = null!;
    private Visitor visitor2 = null!;
    private Attraction attraction1 = null!;
    private Attraction attraction2 = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new ScoreLogRepository(this.context);

        this.visitor1 = new Visitor("John", "Doe", "john@example.com", "password123", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        this.visitor2 = new Visitor("Jane", "Smith", "jane@example.com", "password456", new DateTime(1985, 5, 15), MembershipLevel.Premium, Guid.NewGuid());
        this.attraction1 = new Attraction("Thunder Mountain", "Fast roller coaster", "RollerCoaster", 12, 20);
        this.attraction2 = new Attraction("Haunted House", "Scary experience", "HauntedHouse", 10, 15);

        this.context.Visitors.AddRange(this.visitor1, this.visitor2);
        this.context.Attractions.AddRange(this.attraction1, this.attraction2);
        this.context.SaveChanges();
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ValidScoreLog_SavesSuccessfully()
    {
        var attractionAccessId = Guid.NewGuid();
        var awardedAt = new DateTime(2025, 10, 2, 14, 30, 0);
        var scoreLog = new ScoreLog(this.visitor1.Id, attractionAccessId, this.attraction1.Id, 100, "ScoreByAttractionType", awardedAt);

        var result = this.repository.Add(scoreLog);

        Assert.IsNotNull(result);
        Assert.AreEqual(scoreLog.Id, result.Id);
        Assert.AreEqual(this.visitor1.Id, result.VisitorId);
        Assert.AreEqual(this.attraction1.Id, result.AttractionId);
        Assert.AreEqual(100, result.PointsAwarded);
        Assert.AreEqual("ScoreByAttractionType", result.StrategyUsed);
        Assert.AreEqual(awardedAt, result.AwardedAt);

        var saved = this.context.ScoreLogs.Find(scoreLog.Id);
        Assert.IsNotNull(saved);
    }

    [TestMethod]
    public void Add_MultipleScoreLogs_SavesAll()
    {
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", DateTime.Now);
        var scoreLog2 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", DateTime.Now);

        this.repository.Add(scoreLog1);
        this.repository.Add(scoreLog2);

        var allLogs = this.context.ScoreLogs.ToList();
        Assert.AreEqual(2, allLogs.Count);
    }

    [TestMethod]
    public void GetByVisitor_ExistingScoreLogs_ReturnsCorrectLogs()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", targetDate.AddHours(10));
        var scoreLog2 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", targetDate.AddHours(14));
        var scoreLog3 = new ScoreLog(this.visitor2.Id, Guid.NewGuid(), this.attraction1.Id, 70, "ScoreByCombo", targetDate.AddHours(12));

        this.context.ScoreLogs.AddRange(scoreLog1, scoreLog2, scoreLog3);
        this.context.SaveChanges();

        var result = this.repository.GetByVisitor(this.visitor1.Id);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(sl => sl.VisitorId == this.visitor1.Id));
        Assert.IsTrue(result.All(sl => sl.AwardedAt.Date == targetDate.Date));
    }

    [TestMethod]
    public void GetByVisitorAndDate_NoScoreLogs_ReturnsEmpty()
    {
        var result = this.repository.GetByVisitor(this.visitor1.Id);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetByVisitor_ReturnsOrderedByTime()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", targetDate.AddHours(14));
        var scoreLog2 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", targetDate.AddHours(10));
        var scoreLog3 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 70, "ScoreByCombo", targetDate.AddHours(16));

        this.context.ScoreLogs.AddRange(scoreLog1, scoreLog2, scoreLog3);
        this.context.SaveChanges();

        var result = this.repository.GetByVisitor(this.visitor1.Id).ToList();

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result[0].AwardedAt < result[1].AwardedAt);
        Assert.IsTrue(result[1].AwardedAt < result[2].AwardedAt);
    }

    [TestMethod]
    public void GetByDate_ExistingScoreLogs_ReturnsAllForDate()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", targetDate.AddHours(10));
        var scoreLog2 = new ScoreLog(this.visitor2.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", targetDate.AddHours(14));
        var scoreLog3 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 70, "ScoreByCombo", new DateTime(2025, 10, 3).AddHours(10));

        this.context.ScoreLogs.AddRange(scoreLog1, scoreLog2, scoreLog3);
        this.context.SaveChanges();

        var result = this.repository.GetByDate(targetDate);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(sl => sl.AwardedAt.Date == targetDate.Date));
    }

    [TestMethod]
    public void GetByDate_NoScoreLogs_ReturnsEmpty()
    {
        var result = this.repository.GetByDate(DateTime.Today);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetByDate_IncludesDifferentVisitors()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", targetDate.AddHours(10));
        var scoreLog2 = new ScoreLog(this.visitor2.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", targetDate.AddHours(14));

        this.context.ScoreLogs.AddRange(scoreLog1, scoreLog2);
        this.context.SaveChanges();

        var result = this.repository.GetByDate(targetDate).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(sl => sl.VisitorId == this.visitor1.Id));
        Assert.IsTrue(result.Any(sl => sl.VisitorId == this.visitor2.Id));
    }

    [TestMethod]
    public void GetByDate_IgnoresTimeComponent()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction1.Id, 100, "ScoreByAttractionType", targetDate.AddHours(0));
        var scoreLog2 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), this.attraction2.Id, 80, "ScoreByAttractionType", targetDate.AddHours(23).AddMinutes(59));

        this.context.ScoreLogs.AddRange(scoreLog1, scoreLog2);
        this.context.SaveChanges();

        var result = this.repository.GetByDate(targetDate.AddHours(12));

        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void TotalPointsByVisitor_ReturnsSumOfPoints_ForGivenVisitor()
    {
        var scoreLog1 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), Guid.NewGuid(), 100, "StrategyA", DateTime.UtcNow);
        var scoreLog2 = new ScoreLog(this.visitor1.Id, Guid.NewGuid(), Guid.NewGuid(), 80, "StrategyB", DateTime.UtcNow);
        var scoreLogOther = new ScoreLog(this.visitor2.Id, Guid.NewGuid(), Guid.NewGuid(), 50, "StrategyA", DateTime.UtcNow);

        this.repository.Add(scoreLog1);
        this.repository.Add(scoreLog2);
        this.repository.Add(scoreLogOther);

        var total = this.repository.TotalPointsByVisitor(this.visitor1.Id);

        Assert.AreEqual(180, total);
    }

    [TestMethod]
    public void TotalPointsByVisitor_ReturnsZero_WhenNoLogsForVisitor()
    {
        // No score logs added for visitor2 in this test
        var total = this.repository.TotalPointsByVisitor(this.visitor2.Id);

        Assert.AreEqual(0, total);
    }
}
