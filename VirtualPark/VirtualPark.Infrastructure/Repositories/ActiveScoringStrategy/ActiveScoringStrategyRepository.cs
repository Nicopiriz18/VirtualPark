// <copyright file="ActiveScoringStrategyRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class ActiveScoringStrategyRepository(ParkDbContext context) : IActiveScoringStrategyRepository
{
    private readonly ParkDbContext context = context;

    public ActiveScoringStrategy? GetActive()
    {
        return this.context.ActiveScoringStrategies.FirstOrDefault(a => a.Id == 1);
    }

    public void SetActive(string strategyName)
    {
        var activeStrategy = this.context.ActiveScoringStrategies.FirstOrDefault(a => a.Id == 1);
        if (activeStrategy != null)
        {
            activeStrategy.StrategyName = strategyName;
            this.context.SaveChanges();
        }
        else
        {
            this.context.ActiveScoringStrategies.Add(new VirtualPark.Domain.ActiveScoringStrategy { Id = 1, StrategyName = strategyName });
            this.context.SaveChanges();
        }
    }
}
