// <copyright file="RewardService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using IRepositories;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Scoring;

public class RewardService(
    IRewardRepository rewardRepo,
    IVisitorRepository visitorRepo,
    IScoreLogRepository scoreLogRepo,
    IClockService clock,
    IRewardRedemptionRepository redemptionRepo) : IRewardService
{
    private readonly IVisitorRepository visitorRepo = visitorRepo;
    private readonly IRewardRepository rewardRepo = rewardRepo;
    private readonly IScoreLogRepository scoreLogRepo = scoreLogRepo;
    private readonly IClockService clock = clock;
    private readonly IRewardRedemptionRepository redemptionRepo = redemptionRepo;

    public Reward CreateReward(string name, string description, int costInPoints, int availableQuantity, MembershipLevel? requiredLevel)
    {
        var reward = new Reward(name, description, costInPoints, availableQuantity, requiredLevel);
        this.rewardRepo.Add(reward);
        return reward;
    }

    public RewardRedemption Redeem(Guid visitorId, Guid rewardId)
    {
        var visitor = this.visitorRepo.GetById(visitorId) ?? throw new KeyNotFoundException("Visitor not found");
        var reward = this.rewardRepo.GetById(rewardId) ?? throw new KeyNotFoundException("Reward not found");

        var totalPoints = this.scoreLogRepo.TotalPointsByVisitor(visitorId);
        if (reward.RequiredLevel.HasValue && visitor.MembershipLevel < reward.RequiredLevel)
        {
            throw new InvalidOperationException("Membership level not sufficient");
        }

        if (totalPoints < reward.CostInPoints)
        {
            throw new InvalidOperationException("Insufficient points");
        }

        reward.DecreaseStock();

        // Registrar canje (histórico)
        var redemption = new RewardRedemption(visitorId, rewardId, reward.CostInPoints, this.clock.GetNow());

        // Registrar descuento como log negativo
        var log = ScoreLog.CreateForRewardRedemption(visitorId, -reward.CostInPoints, "RewardRedemption", this.clock.GetNow());

        this.rewardRepo.Update(reward);
        this.scoreLogRepo.Add(log);
        this.redemptionRepo.Add(redemption);

        return redemption;
    }

    public IEnumerable<Reward> GetAllRewards()
    {
        return this.rewardRepo.GetAll();
    }
}
