// <copyright file="ScoreLogRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories.ScoreLogs;

public class ScoreLogRepository(ParkDbContext context) : IScoreLogRepository
{
    private readonly ParkDbContext context = context;

    public ScoreLog Add(ScoreLog scoreLog)
    {
        this.context.ScoreLogs.Add(scoreLog);
        this.context.SaveChanges();
        return scoreLog;
    }

    public IEnumerable<ScoreLog> GetByVisitor(Guid visitorId)
    {
        return this.context.ScoreLogs
            .AsNoTracking()
            .Where(sl => sl.VisitorId == visitorId)
            .OrderBy(sl => sl.AwardedAt)
            .ToList();
    }

    public int TotalPointsByVisitor(Guid visitorId)
    {
        return this.context.ScoreLogs
            .AsNoTracking()
            .Where(sl => sl.VisitorId == visitorId)
            .Sum(sl => sl.PointsAwarded);
    }

    public IEnumerable<ScoreLog> GetByDate(DateTime date)
    {
        return this.context.ScoreLogs
            .AsNoTracking()
            .Where(sl => sl.AwardedAt.Date == date.Date && sl.PointsAwarded > 0 && sl.StrategyUsed != "RewardRedemption")
            .OrderBy(sl => sl.AwardedAt)
            .ToList();
    }
}
