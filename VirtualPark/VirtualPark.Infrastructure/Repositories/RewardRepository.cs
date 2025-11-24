// <copyright file="RewardRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using IRepositories;
using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class RewardRepository : IRewardRepository
{
    private readonly ParkDbContext context;

    public RewardRepository(ParkDbContext context)
    {
        this.context = context;
    }

    public void Add(Reward reward)
    {
        this.context.Rewards.Add(reward);
        this.context.SaveChanges();
    }

    public Reward? GetById(Guid id)
    {
        return this.context.Rewards.Find(id);
    }

    public IEnumerable<Reward> GetAll()
    {
        return this.context.Rewards.AsNoTracking().ToList();
    }

    public void Update(Reward reward)
    {
        var existingReward = this.context.Rewards.Find(reward.Id);
        if (existingReward != null)
        {
            // Copia los valores del objeto entrante al existente
            this.context.Entry(existingReward).CurrentValues.SetValues(reward);
            this.context.SaveChanges();
        }
    }
}
