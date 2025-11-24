// <copyright file="RewardRedemption.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class RewardRedemption(Guid visitorId, Guid rewardId, int pointsSpent, DateTime date)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid VisitorId { get; private set; } = visitorId;

    public Guid RewardId { get; private set; } = rewardId;

    public DateTime Date { get; private set; } = date;

    public int PointsSpent { get; private set; } = pointsSpent;
}
