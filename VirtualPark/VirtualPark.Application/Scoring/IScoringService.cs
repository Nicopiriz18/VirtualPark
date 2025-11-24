// <copyright file="IScoringService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Scoring;

public interface IScoringService
{
    void AwardPoints(Guid visitorId, Guid attractionAccessId, Guid attractionId, DateTime awardedAt);

    IEnumerable<DailyRankingEntry> GetDailyRanking(DateTime date, int top = 10);

    string GetActiveStrategy();

    void SetActiveStrategy(string strategyName);

    IEnumerable<string> GetAvailableStrategies();
}
