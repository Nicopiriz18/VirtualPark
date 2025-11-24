// <copyright file="RewardRedemptionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class RewardRedemptionRepository(ParkDbContext context) : IRewardRedemptionRepository
{
    private readonly ParkDbContext context = context;

    public RewardRedemption Add(RewardRedemption redemption)
    {
        this.context.RewardRedemptions.Add(redemption);
        this.context.SaveChanges();
        return redemption;
    }

    public IEnumerable<RewardRedemption> GetByVisitor(Guid visitorId)
    {
        return this.context.RewardRedemptions
            .AsNoTracking()
            .Where(rr => rr.VisitorId == visitorId)
            .OrderByDescending(rr => rr.Date)
            .ToList();
    }
}
