// <copyright file="ScoreHistoryServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Scoring;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.DTOs.Scoring.Responses;

namespace VirtualPark.Application.Tests;
[TestClass]
public class ScoreHistoryServiceTests
{
    private readonly Mock<IScoreLogRepository> mockScoreLogRepo;
    private readonly ScoreHistoryService service;

    public ScoreHistoryServiceTests()
    {
        this.mockScoreLogRepo = new Mock<IScoreLogRepository>();
        this.service = new ScoreHistoryService(this.mockScoreLogRepo.Object);
    }

    [TestMethod]
    public void GetVisitorHistory_ReturnsCorrectHistory()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var scoreLogs = new List<ScoreLog>
        {
            new ScoreLog(visitorId, Guid.NewGuid(), Guid.NewGuid(), 100, "ScoreByAttractionType", DateTime.UtcNow),
            new ScoreLog(visitorId, Guid.NewGuid(), Guid.NewGuid(), -50, "RewardRedemption", DateTime.UtcNow.AddDays(-1)),
        };

        this.mockScoreLogRepo.Setup(repo => repo.GetByVisitor(visitorId)).Returns(scoreLogs);

        // Act
        var result = this.service.GetVisitorHistory(visitorId).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(100, result[0].PointsAwarded);
        Assert.AreEqual("ScoreByAttractionType", result[0].StrategyUsed);
        Assert.AreEqual("Atracción", result[0].Origin);
        Assert.AreEqual(-50, result[1].PointsAwarded);
        Assert.AreEqual("RewardRedemption", result[1].StrategyUsed);
        Assert.AreEqual("Canje de Recompensa", result[1].Origin);

        this.mockScoreLogRepo.Verify(repo => repo.GetByVisitor(visitorId), Times.Once);
    }
}
