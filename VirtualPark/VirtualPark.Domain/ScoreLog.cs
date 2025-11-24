// <copyright file="ScoreLog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class ScoreLog
{
    public Guid Id { get; private set; }

    public Guid VisitorId { get; private set; }

    public Guid? AttractionAccessId { get; private set; }

    public Guid? AttractionId { get; private set; }

    public int PointsAwarded { get; private set; }

    public string StrategyUsed { get; private set; }

    public DateTime AwardedAt { get; private set; }

    private ScoreLog()
    {
        this.StrategyUsed = string.Empty;
    }

    public ScoreLog(
        Guid visitorId,
        Guid attractionAccessId,
        Guid attractionId,
        int pointsAwarded,
        string strategyUsed,
        DateTime awardedAt)
    {
        ValidateGuid(visitorId, nameof(visitorId));
        ValidateGuid(attractionAccessId, nameof(attractionAccessId));
        ValidateGuid(attractionId, nameof(attractionId));
        ValidateStrategyName(strategyUsed);

        this.Id = Guid.NewGuid();
        this.VisitorId = visitorId;
        this.AttractionAccessId = attractionAccessId;
        this.AttractionId = attractionId;
        this.PointsAwarded = pointsAwarded;
        this.StrategyUsed = strategyUsed;
        this.AwardedAt = awardedAt;
    }

    public static ScoreLog CreateForRewardRedemption(Guid visitorId, int pointsAwarded, string strategyUsed, DateTime awardedAt)
    {
        ValidateGuid(visitorId, nameof(visitorId));
        ValidateStrategyName(strategyUsed);

        var log = new ScoreLog();
        log.Id = Guid.NewGuid();
        log.VisitorId = visitorId;
        log.AttractionAccessId = null;
        log.AttractionId = null;
        log.PointsAwarded = pointsAwarded;
        log.StrategyUsed = strategyUsed;
        log.AwardedAt = awardedAt;
        return log;
    }

    private static void ValidateGuid(Guid guid, string paramName)
    {
        if (guid == Guid.Empty)
        {
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }
    }

    private static void ValidatePoints(int points)
    {
        if (points < 0)
        {
            throw new ArgumentException("Points cannot be negative.", nameof(points));
        }
    }

    private static void ValidateStrategyName(string strategyName)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            throw new ArgumentException("Strategy name cannot be null or empty.", nameof(strategyName));
        }
    }
}
