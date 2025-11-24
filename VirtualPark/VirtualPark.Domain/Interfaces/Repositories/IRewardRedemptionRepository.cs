// <copyright file="IRewardRedemptionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IRewardRedemptionRepository
{
    RewardRedemption Add(RewardRedemption redemption);

    IEnumerable<RewardRedemption> GetByVisitor(Guid visitorId);
}
