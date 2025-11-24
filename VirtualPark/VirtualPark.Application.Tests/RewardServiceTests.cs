// <copyright file="RewardServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using IRepositories;
using Moq;
using VirtualPark.Application.Clock;
using VirtualPark.Application.Scoring;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tests;

[TestClass]
public class RewardServiceTests
{
    private Mock<IRewardRepository> rewardRepoMock = null!;
    private Mock<IVisitorRepository> visitorRepoMock = null!;
    private Mock<IScoreLogRepository> scoreLogRepoMock = null!;
    private Mock<IScoringService> scoringServiceMock = null!;
    private Mock<IClockService> clockMock = null!;
    private Mock<IRewardRedemptionRepository> redemptionRepoMock = null!;
    private RewardService service = null!;

    [TestInitialize]
    public void Setup()
    {
        this.rewardRepoMock = new Mock<IRewardRepository>();
        this.visitorRepoMock = new Mock<IVisitorRepository>();
        this.scoreLogRepoMock = new Mock<IScoreLogRepository>();
        this.scoringServiceMock = new Mock<IScoringService>();
        this.clockMock = new Mock<IClockService>();
        this.redemptionRepoMock = new Mock<IRewardRedemptionRepository>();

        this.service = new RewardService(
            this.rewardRepoMock.Object,
            this.visitorRepoMock.Object,
            this.scoreLogRepoMock.Object,
            this.clockMock.Object,
            this.redemptionRepoMock.Object);
    }

    [TestMethod]
    public void CreateReward_AddsAndReturnsReward()
    {
        var result = this.service.CreateReward("Popcorn", "Tasty", 50, 10, MembershipLevel.Standard);

        Assert.IsNotNull(result);
        Assert.AreEqual("Popcorn", result.Name);
        Assert.AreEqual(50, result.CostInPoints);
        this.rewardRepoMock.Verify(r => r.Add(It.Is<Reward>(x => x.Id == result.Id)), Times.Once);
    }

    [TestMethod]
    public void Redeem_Succeeds_WhenConditionsMet()
    {
        var visitor = new Visitor("John", "Doe", "john@e", "pwd", new DateTime(1990, 1, 1), MembershipLevel.Premium, Guid.NewGuid());
        var reward = new Reward("Toy", "Fun", 100, 2, MembershipLevel.Standard);

        this.visitorRepoMock.Setup(v => v.GetById(visitor.Id)).Returns(visitor);
        this.rewardRepoMock.Setup(r => r.GetById(reward.Id)).Returns(reward);
        this.scoreLogRepoMock.Setup(s => s.TotalPointsByVisitor(visitor.Id)).Returns(200);
        var now = DateTime.UtcNow;
        this.clockMock.Setup(c => c.GetNow()).Returns(now);

        var redemption = this.service.Redeem(visitor.Id, reward.Id);

        Assert.IsNotNull(redemption);
        Assert.AreEqual(visitor.Id, redemption.VisitorId);
        Assert.AreEqual(reward.Id, redemption.RewardId);
        Assert.AreEqual(reward.CostInPoints, redemption.PointsSpent);

        this.rewardRepoMock.Verify(r => r.Update(It.Is<Reward>(x => x.Id == reward.Id && x.AvailableQuantity == 1)), Times.Once);
        this.scoreLogRepoMock.Verify(
            s => s.Add(It.Is<ScoreLog>(log =>
            log.VisitorId == visitor.Id &&
            log.PointsAwarded == -reward.CostInPoints &&
            log.StrategyUsed == "RewardRedemption")), Times.Once);
        this.redemptionRepoMock.Verify(r => r.Add(It.Is<RewardRedemption>(rr => rr.VisitorId == visitor.Id && rr.RewardId == reward.Id)), Times.Once);
    }

    [TestMethod]
    public void Redeem_Throws_WhenVisitorNotFound()
    {
        this.visitorRepoMock.Setup(v => v.GetById(It.IsAny<Guid>())).Returns((Visitor?)null);

        Assert.ThrowsException<KeyNotFoundException>(() => this.service.Redeem(Guid.NewGuid(), Guid.NewGuid()));
    }

    [TestMethod]
    public void Redeem_Throws_WhenRewardNotFound()
    {
        var visitor = new Visitor("J", "D", "a@b", "p", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        this.visitorRepoMock.Setup(v => v.GetById(visitor.Id)).Returns(visitor);
        this.rewardRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Reward?)null);

        Assert.ThrowsException<KeyNotFoundException>(() => this.service.Redeem(visitor.Id, Guid.NewGuid()));
    }

    [TestMethod]
    public void Redeem_Throws_WhenMembershipInsufficient()
    {
        var visitor = new Visitor("J", "D", "a@b", "p", new DateTime(1990, 1, 1), MembershipLevel.Standard, Guid.NewGuid());
        var reward = new Reward("VIP", "Only for premium", 10, 1, MembershipLevel.Premium);

        this.visitorRepoMock.Setup(v => v.GetById(visitor.Id)).Returns(visitor);
        this.rewardRepoMock.Setup(r => r.GetById(reward.Id)).Returns(reward);
        this.scoreLogRepoMock.Setup(s => s.TotalPointsByVisitor(visitor.Id)).Returns(100);

        Assert.ThrowsException<InvalidOperationException>(() => this.service.Redeem(visitor.Id, reward.Id));
    }

    [TestMethod]
    public void Redeem_Throws_WhenInsufficientPoints()
    {
        var visitor = new Visitor("J", "D", "a@b", "p", new DateTime(1990, 1, 1), MembershipLevel.Premium, Guid.NewGuid());
        var reward = new Reward("Expensive", "Costly", 500, 1, null);

        this.visitorRepoMock.Setup(v => v.GetById(visitor.Id)).Returns(visitor);
        this.rewardRepoMock.Setup(r => r.GetById(reward.Id)).Returns(reward);
        this.scoreLogRepoMock.Setup(s => s.TotalPointsByVisitor(visitor.Id)).Returns(100);

        Assert.ThrowsException<InvalidOperationException>(() => this.service.Redeem(visitor.Id, reward.Id));
    }

    [TestMethod]
    public void Redeem_Throws_WhenNoStock()
    {
        var visitor = new Visitor("J", "D", "a@b", "p", new DateTime(1990, 1, 1), MembershipLevel.Premium, Guid.NewGuid());
        var reward = new Reward("Gone", "No stock", 10, 0, null);

        this.visitorRepoMock.Setup(v => v.GetById(visitor.Id)).Returns(visitor);
        this.rewardRepoMock.Setup(r => r.GetById(reward.Id)).Returns(reward);
        this.scoreLogRepoMock.Setup(s => s.TotalPointsByVisitor(visitor.Id)).Returns(100);

        Assert.ThrowsException<InvalidOperationException>(() => this.service.Redeem(visitor.Id, reward.Id));
    }
}
