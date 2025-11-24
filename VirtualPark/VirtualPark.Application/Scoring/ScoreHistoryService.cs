// <copyright file="ScoreHistoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.DTOs.Scoring.Responses;

namespace VirtualPark.Application.Scoring;

public class ScoreHistoryService : IScoreHistoryService
{
    private readonly IScoreLogRepository scoreLogRepo;

    public ScoreHistoryService(IScoreLogRepository scoreLogRepo)
    {
        this.scoreLogRepo = scoreLogRepo;
    }

    public IEnumerable<ScoreHistoryEntryDto> GetVisitorHistory(Guid visitorId)
    {
        var scoreLogs = this.scoreLogRepo.GetByVisitor(visitorId);

        return scoreLogs.Select(log => new ScoreHistoryEntryDto
        {
            Id = log.Id,
            Date = log.AwardedAt,
            PointsAwarded = log.PointsAwarded,
            Origin = log.StrategyUsed == "RewardRedemption" ? "Canje de Recompensa" : "Atracción",
            StrategyUsed = log.StrategyUsed,
            AttractionName = log.AttractionId != Guid.Empty ? $"Atracción {log.AttractionId}" : null,
            Description = log.StrategyUsed == "RewardRedemption"
                ? $"Canje de recompensa ({Math.Abs(log.PointsAwarded)} puntos)"
                : $"Puntos ganados: {log.PointsAwarded}",
        });
    }
}
