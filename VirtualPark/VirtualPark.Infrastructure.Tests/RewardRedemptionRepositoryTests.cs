// <copyright file="RewardRedemptionRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class RewardRedemptionRepositoryTests
{
    private RewardRedemptionRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new RewardRedemptionRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldPersistRedemption()
    {
        var redemption = new RewardRedemption(Guid.NewGuid(), Guid.NewGuid(), 50, DateTime.UtcNow);

        var saved = this.repository.Add(redemption);

        Assert.IsNotNull(saved);
        Assert.AreEqual(redemption.Id, saved.Id);
        Assert.AreEqual(50, saved.PointsSpent);
    }

    [TestMethod]
    public void GetByVisitor_ShouldReturnAllEntriesOrderedDescendingByDate()
    {
        var visitorId = Guid.NewGuid();
        var early = new RewardRedemption(visitorId, Guid.NewGuid(), 25, DateTime.UtcNow.AddDays(-1));
        var later = new RewardRedemption(visitorId, Guid.NewGuid(), 30, DateTime.UtcNow);

        this.repository.Add(early);
        this.repository.Add(later);

        var list = this.repository.GetByVisitor(visitorId).ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(later.Id, list[0].Id);
        Assert.AreEqual(early.Id, list[1].Id);
    }

    [TestMethod]
    public void GetByVisitor_ShouldReturnEmpty_WhenNone()
    {
        var result = this.repository.GetByVisitor(Guid.NewGuid());
        Assert.IsFalse(result.Any());
    }
}
