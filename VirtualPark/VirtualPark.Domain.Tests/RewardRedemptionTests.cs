// <copyright file="RewardRedemptionTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace Domain.Tests;

[TestClass]
public class RewardRedemptionTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        var visitorId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var date = new DateTime(2025, 10, 21, 12, 0, 0, DateTimeKind.Utc);
        var points = 150;

        var redemption = new RewardRedemption(visitorId, rewardId, points, date);

        Assert.AreNotEqual(Guid.Empty, redemption.Id);
        Assert.AreEqual(visitorId, redemption.VisitorId);
        Assert.AreEqual(rewardId, redemption.RewardId);
        Assert.AreEqual(points, redemption.PointsSpent);
        Assert.AreEqual(date, redemption.Date);
    }

    [TestMethod]
    public void MultipleInstances_HaveDifferentIds_ButSameData()
    {
        var visitorId = Guid.NewGuid();
        var rewardId = Guid.NewGuid();
        var date = DateTime.UtcNow;
        var points = 50;

        var r1 = new RewardRedemption(visitorId, rewardId, points, date);
        var r2 = new RewardRedemption(visitorId, rewardId, points, date);

        Assert.AreNotEqual(r1.Id, r2.Id);
        Assert.AreEqual(r1.VisitorId, r2.VisitorId);
        Assert.AreEqual(r1.RewardId, r2.RewardId);
        Assert.AreEqual(r1.PointsSpent, r2.PointsSpent);
        Assert.AreEqual(r1.Date, r2.Date);
    }
}
