// <copyright file="ScoringService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Scoring;

public class ScoringService : IScoringService
{
    private readonly IScoreLogRepository scoreLogRepo;
    private readonly IVisitorRepository visitorRepo;
    private readonly IAttractionRepository attractionRepo;
    private readonly IAttractionAccessRepository accessRepo;
    private readonly ISpecialEventRepository eventRepo;
    private readonly IScoringStrategyLoader strategyLoader;
    private readonly IActiveScoringStrategyRepository strategyRepo;
    private readonly string defaultStrategy = "ScoreByAttractionType";
    private string activeStrategyName;

    public ScoringService(
        IScoreLogRepository scoreLogRepo,
        IVisitorRepository visitorRepo,
        IAttractionRepository attractionRepo,
        IAttractionAccessRepository accessRepo,
        ISpecialEventRepository eventRepo,
        IScoringStrategyLoader strategyLoader,
        IActiveScoringStrategyRepository strategyRepo)
    {
        this.scoreLogRepo = scoreLogRepo;
        this.visitorRepo = visitorRepo;
        this.attractionRepo = attractionRepo;
        this.accessRepo = accessRepo;
        this.eventRepo = eventRepo;
        this.strategyLoader = strategyLoader;
        this.strategyRepo = strategyRepo;
        this.activeStrategyName = this.strategyRepo.GetActive()?.StrategyName ?? this.defaultStrategy;
    }

    public void AwardPoints(Guid visitorId, Guid attractionAccessId, Guid attractionId, DateTime awardedAt)
    {
        Console.WriteLine($"Awarding points for visitor {visitorId} on attraction {attractionId} at {awardedAt}");
        var visitor = this.visitorRepo.GetById(visitorId) ?? throw new KeyNotFoundException("Visitor not found");
        var attraction = this.attractionRepo.GetById(attractionId) ?? throw new KeyNotFoundException("Attraction not found");
        var todayAccesses = this.accessRepo.GetAccessesByVisitorAndDate(visitorId, awardedAt.Date);
        var activeEvent = this.GetActiveEventForDate(awardedAt.Date);

        var activeStrategyName = this.GetActiveStrategy();
        var strategies = this.strategyLoader.GetStrategies();

        if (!strategies.TryGetValue(activeStrategyName, out var strategy))
        {
            var inCodeNamespace = typeof(ScoreByAttractionTypeStrategy).Namespace!;
            strategy = strategies.Values.FirstOrDefault(s =>
                s.GetType().Namespace is string ns &&
                ns.StartsWith(inCodeNamespace, StringComparison.Ordinal));

            if (strategy is null)
            {
                throw new KeyNotFoundException(
                    $"Strategy '{activeStrategyName}' not found and no in-code strategies are available");
            }

            activeStrategyName = strategy.Name;
            this.strategyRepo.SetActive(activeStrategyName);
        }

        Console.WriteLine($"Using strategy {activeStrategyName}");
        var points = strategy.CalculatePoints(attraction, visitor, todayAccesses, activeEvent);
        Console.WriteLine($"Calculated points: {points}");

        var scoreLog = new ScoreLog(visitorId, attractionAccessId, attractionId, points, activeStrategyName, awardedAt);
        this.scoreLogRepo.Add(scoreLog);
        Console.WriteLine($"Added score log for visitor {visitorId} on attraction {attractionId} at {awardedAt}");
    }

    public IEnumerable<DailyRankingEntry> GetDailyRanking(DateTime date, int top = 10)
    {
        var scoreLogs = this.scoreLogRepo.GetByDate(date);

        var groupedByVisitor = scoreLogs
            .GroupBy(sl => sl.VisitorId)
            .Select(g => new
            {
                VisitorId = g.Key,
                TotalPoints = g.Sum(sl => sl.PointsAwarded),
            })
            .OrderByDescending(x => x.TotalPoints)
            .Take(top)
            .ToList();

        var result = new List<DailyRankingEntry>();
        var position = 1;

        foreach (var entry in groupedByVisitor)
        {
            var visitor = this.visitorRepo.GetById(entry.VisitorId);
            if (visitor != null)
            {
                result.Add(new DailyRankingEntry
                {
                    VisitorId = entry.VisitorId,
                    VisitorName = $"{visitor.Name} {visitor.Surname}",
                    TotalPoints = entry.TotalPoints,
                    Position = position++,
                });
            }
        }

        return result;
    }

    public string GetActiveStrategy()
    {
        var activeStrategy = this.strategyRepo.GetActive();
        if (!string.IsNullOrWhiteSpace(activeStrategy?.StrategyName))
        {
            this.activeStrategyName = activeStrategy.StrategyName;
            return activeStrategy.StrategyName;
        }

        return this.activeStrategyName;
    }

    public void SetActiveStrategy(string strategyName)
    {
        var strategies = this.strategyLoader.GetStrategies();
        if (!strategies.ContainsKey(strategyName))
        {
            throw new ArgumentException($"Strategy '{strategyName}' not found", nameof(strategyName));
        }

        this.strategyRepo.SetActive(strategyName);
        this.activeStrategyName = strategyName;
    }

    public IEnumerable<string> GetAvailableStrategies()
    {
        var strategies = this.strategyLoader.GetStrategies();
        return strategies.Keys;
    }

    private Domain.SpecialEvent? GetActiveEventForDate(DateTime date)
    {
        var allEvents = this.eventRepo.GetAll();
        return allEvents.FirstOrDefault(e => e.Date.Date == date.Date);
    }
}
