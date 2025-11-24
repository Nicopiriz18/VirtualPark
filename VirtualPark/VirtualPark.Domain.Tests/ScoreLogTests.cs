// <copyright file="ScoreLogTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class ScoreLogTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_CreatesScoreLog()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        var strategyUsed = "ScoreByAttractionType";
        var awardedAt = DateTime.Now;

        // Act
        var scoreLog = new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt);

        // Assert
        Assert.IsNotNull(scoreLog);
        Assert.AreNotEqual(Guid.Empty, scoreLog.Id);
        Assert.AreEqual(visitorId, scoreLog.VisitorId);
        Assert.AreEqual(attractionAccessId, scoreLog.AttractionAccessId);
        Assert.AreEqual(attractionId, scoreLog.AttractionId);
        Assert.AreEqual(pointsAwarded, scoreLog.PointsAwarded);
        Assert.AreEqual(strategyUsed, scoreLog.StrategyUsed);
        Assert.AreEqual(awardedAt, scoreLog.AwardedAt);
    }

    [TestMethod]
    public void Constructor_GeneratesUniqueId()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        var strategyUsed = "ScoreByAttractionType";
        var awardedAt = DateTime.Now;

        // Act
        var scoreLog1 = new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt);
        var scoreLog2 = new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt);

        // Assert
        Assert.AreNotEqual(scoreLog1.Id, scoreLog2.Id);
    }

    [TestMethod]
    public void Constructor_WithNullStrategyName_ThrowsArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        string strategyUsed = null!;
        var awardedAt = DateTime.Now;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt));
    }

    [TestMethod]
    public void Constructor_WithEmptyStrategyName_ThrowsArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        var strategyUsed = string.Empty;
        var awardedAt = DateTime.Now;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt));
    }

    [TestMethod]
    public void Constructor_WithEmptyVisitorId_ThrowsArgumentException()
    {
        // Arrange
        var visitorId = Guid.Empty;
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        var strategyUsed = "ScoreByAttractionType";
        var awardedAt = DateTime.Now;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt));
    }

    [TestMethod]
    public void Constructor_WithEmptyAttractionAccessId_ThrowsArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.Empty;
        var attractionId = Guid.NewGuid();
        var pointsAwarded = 100;
        var strategyUsed = "ScoreByAttractionType";
        var awardedAt = DateTime.Now;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt));
    }

    [TestMethod]
    public void Constructor_WithEmptyAttractionId_ThrowsArgumentException()
    {
        // Arrange
        var visitorId = Guid.NewGuid();
        var attractionAccessId = Guid.NewGuid();
        var attractionId = Guid.Empty;
        var pointsAwarded = 100;
        var strategyUsed = "ScoreByAttractionType";
        var awardedAt = DateTime.Now;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ScoreLog(visitorId, attractionAccessId, attractionId, pointsAwarded, strategyUsed, awardedAt));
    }
}
