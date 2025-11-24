// <copyright file="ScoreMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Scoring;
using VirtualPark.DTOs.Scoring.Requests;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class ScoreMappingExtensionsTests
{
    [TestMethod]
    public void ToDto_ScoreLog_ShouldMapScoreLogToDtoCorrectly()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var accessId = Guid.NewGuid();
        var awardedAt = new DateTime(2025, 10, 6, 12, 0, 0);
        var scoreLog = new ScoreLog(visitorId, accessId, attractionId, 100, "ByAttractionType", awardedAt);

        // Act
        var dto = scoreLog.ToDto("John Doe", "Roller Coaster");

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(scoreLog.Id, dto.Id);
        Assert.AreEqual(visitorId, dto.VisitorId);
        Assert.AreEqual(accessId, dto.AttractionAccessId);
        Assert.AreEqual(attractionId, dto.AttractionId);
        Assert.AreEqual(100, dto.PointsAwarded);
        Assert.AreEqual("ByAttractionType", dto.StrategyUsed);
        Assert.AreEqual(awardedAt, dto.AwardedAt);
        Assert.AreEqual("John Doe", dto.VisitorName);
        Assert.AreEqual("Roller Coaster", dto.AttractionName);
    }

    [TestMethod]
    public void ToDto_ScoreLog_ShouldHandleNullNames()
    {
        // Arrange
        var scoreLog = new ScoreLog(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            "Combo",
            DateTime.Now);

        // Act
        var dto = scoreLog.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsNull(dto.VisitorName);
        Assert.IsNull(dto.AttractionName);
    }

    [TestMethod]
    public void ToDto_ActiveScoringStrategy_ShouldMapActiveScoringStrategyToDtoCorrectly()
    {
        // Arrange
        var activatedAt = new DateTime(2025, 10, 1);
        var availableStrategies = new List<string> { "ByAttractionType", "Combo", "EventMultiplier" };
        var strategy = new ActiveScoringStrategy
        {
            StrategyName = "ByAttractionType",
        };

        // Act
        var dto = strategy.ToDto("Awards points based on attraction type", activatedAt, availableStrategies);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(strategy.Id, dto.Id);
        Assert.AreEqual("ByAttractionType", dto.StrategyName);
        Assert.AreEqual("Awards points based on attraction type", dto.Description);
        Assert.AreEqual(activatedAt, dto.ActivatedAt);
        Assert.IsNotNull(dto.AvailableStrategies);
        Assert.AreEqual(3, dto.AvailableStrategies.Count);
    }

    [TestMethod]
    public void ToDto_ActiveScoringStrategy_ShouldHandleNullOptionalFields()
    {
        // Arrange
        var strategy = new ActiveScoringStrategy
        {
            StrategyName = "Combo",
        };

        // Act
        var dto = strategy.ToDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Combo", dto.StrategyName);
        Assert.IsNull(dto.Description);
        Assert.IsNull(dto.ActivatedAt);
        Assert.IsNull(dto.AvailableStrategies);
    }

    [TestMethod]
    public void ToVisitorScoreDto_ShouldMapVisitorToVisitorScoreDtoCorrectly()
    {
        // Arrange
        var visitor = new Visitor(
            "Alice",
            "Johnson",
            "alice@example.com",
            "Password123!",
            new DateTime(1990, 1, 1),
            MembershipLevel.Premium,
            Guid.NewGuid());
        var lastActivity = new DateTime(2025, 10, 6, 15, 30, 0);

        // Act
        var dto = visitor.ToVisitorScoreDto(500, 1, 25, lastActivity);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(visitor.Id, dto.VisitorId);
        Assert.AreEqual("Alice Johnson", dto.VisitorName);
        Assert.AreEqual(500, dto.TotalPoints);
        Assert.AreEqual(1, dto.Position);
        Assert.AreEqual(25, dto.AccessCount);
        Assert.AreEqual(lastActivity, dto.LastActivity);
    }

    [TestMethod]
    public void ToVisitorScoreDto_ShouldHandleNullLastActivity()
    {
        // Arrange
        var visitor = new Visitor(
            "Bob",
            "Smith",
            "bob@example.com",
            "Password123!",
            new DateTime(1995, 5, 5),
            MembershipLevel.Standard,
            Guid.NewGuid());

        // Act
        var dto = visitor.ToVisitorScoreDto(250, 3);

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("Bob Smith", dto.VisitorName);
        Assert.AreEqual(250, dto.TotalPoints);
        Assert.AreEqual(3, dto.Position);
        Assert.AreEqual(0, dto.AccessCount);
        Assert.IsNull(dto.LastActivity);
    }

    [TestMethod]
    public void ToDto_ScoreLogCollection_ShouldHandleNullFunctions()
    {
        // Arrange
        var scoreLogs = new List<ScoreLog>
        {
            new ScoreLog(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 50, "Strategy1", DateTime.Now),
        };

        // Act
        // No pasamos las funciones (quedan en null)
        var dtos = scoreLogs.ToDto();

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(1, dtos.Count);
        Assert.AreEqual(50, dtos[0].PointsAwarded);
        Assert.IsNull(dtos[0].VisitorName);
        Assert.IsNull(dtos[0].AttractionName);
    }

    [TestMethod]
    public void ToDto_ScoreLogCollection_ShouldMapListOfScoreLogsToListOfDtos()
    {
        // Arrange
        var scoreLogs = new List<ScoreLog>
        {
            new ScoreLog(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100, "Strategy1", DateTime.Now),
            new ScoreLog(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 200, "Strategy2", DateTime.Now.AddHours(1)),
        };

        Func<ScoreLog, string?> getVisitorName = sl => "Visitor";
        Func<ScoreLog, string?> getAttractionName = sl => "Attraction";

        // Act
        var dtos = scoreLogs.ToDto(getVisitorName, getAttractionName);

        // Assert
        Assert.IsNotNull(dtos);
        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual(100, dtos[0].PointsAwarded);
        Assert.AreEqual(200, dtos[1].PointsAwarded);
        Assert.AreEqual("Visitor", dtos[0].VisitorName);
        Assert.AreEqual("Attraction", dtos[1].AttractionName);
    }

    [TestMethod]
    public void ToDomain_ShouldMapSetActiveScoringStrategyRequestDtoToActiveScoringStrategy()
    {
        // Arrange
        var dto = new SetActiveScoringStrategyRequestDto
        {
            StrategyName = "EventMultiplier",
        };

        // Act
        var strategy = dto.ToDomain();

        // Assert
        Assert.IsNotNull(strategy);
        Assert.AreEqual("EventMultiplier", strategy.StrategyName);
    }
}
