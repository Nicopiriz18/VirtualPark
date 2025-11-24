// <copyright file="ScoreMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Scoring.Requests;
using VirtualPark.DTOs.Scoring.Responses;

namespace VirtualPark.DTOs.Scoring;

public static class ScoreMappingExtensions
{
    public static ScoreLogDto ToDto(this ScoreLog scoreLog, string? visitorName = null, string? attractionName = null)
    {
        return new ScoreLogDto
        {
            Id = scoreLog.Id,
            VisitorId = scoreLog.VisitorId,
            AttractionAccessId = scoreLog.AttractionAccessId,
            AttractionId = scoreLog.AttractionId,
            PointsAwarded = scoreLog.PointsAwarded,
            StrategyUsed = scoreLog.StrategyUsed,
            AwardedAt = scoreLog.AwardedAt,
            VisitorName = visitorName,
            AttractionName = attractionName,
        };
    }

    public static ActiveScoringStrategyDto ToDto(
        this ActiveScoringStrategy strategy,
        string? description = null,
        DateTime? activatedAt = null,
        List<string>? availableStrategies = null)
    {
        return new ActiveScoringStrategyDto
        {
            Id = strategy.Id,
            StrategyName = strategy.StrategyName,
            Description = description,
            ActivatedAt = activatedAt,
            AvailableStrategies = availableStrategies,
        };
    }

    public static VisitorScoreDto ToVisitorScoreDto(
        this Visitor visitor,
        int totalPoints,
        int position,
        int accessCount = 0,
        DateTime? lastActivity = null)
    {
        return new VisitorScoreDto
        {
            VisitorId = visitor.Id,
            VisitorName = $"{visitor.Name} {visitor.Surname}",
            TotalPoints = totalPoints,
            Position = position,
            AccessCount = accessCount,
            LastActivity = lastActivity,
        };
    }

    public static List<ScoreLogDto> ToDto(
        this IEnumerable<ScoreLog> scoreLogs,
        Func<ScoreLog, string?>? getVisitorName = null,
        Func<ScoreLog, string?>? getAttractionName = null)
    {
        return scoreLogs.Select(sl => sl.ToDto(
            getVisitorName?.Invoke(sl),
            getAttractionName?.Invoke(sl))).ToList();
    }

    public static ActiveScoringStrategy ToDomain(this SetActiveScoringStrategyRequestDto dto)
    {
        return new ActiveScoringStrategy
        {
            StrategyName = dto.StrategyName,
        };
    }
}
