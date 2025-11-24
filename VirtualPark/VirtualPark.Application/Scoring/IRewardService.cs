// <copyright file="IRewardService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Scoring;

public interface IRewardService
{
    Reward CreateReward(string name, string description, int costInPoints, int availableQuantity,
        MembershipLevel? requiredLevel);

    RewardRedemption Redeem(Guid visitorId, Guid rewardId);

    IEnumerable<Reward> GetAllRewards();
}
