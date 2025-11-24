// <copyright file="RewardRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class RewardRepositoryTests
{
    private RewardRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new RewardRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldStoreReward_AndGetByIdReturnsIt()
    {
        var reward = new Reward("Candy", "Sweet", 10, 5, null);

        this.repository.Add(reward);
        var found = this.repository.GetById(reward.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(reward.Id, found!.Id);
        Assert.AreEqual("Candy", found.Name);
        Assert.AreEqual(5, found.AvailableQuantity);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllAddedRewards()
    {
        var r1 = new Reward("A", "first", 5, 2, null);
        var r2 = new Reward("B", "second", 8, 3, null);

        this.repository.Add(r1);
        this.repository.Add(r2);

        var all = this.repository.GetAll().ToList();

        Assert.AreEqual(2, all.Count);
        CollectionAssert.AreEquivalent(new[] { r1.Id, r2.Id }, all.Select(r => r.Id).ToArray());
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        var result = this.repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldPersistChanges_WhenPassingExistingInstance()
    {
        var reward = new Reward("Toy", "Small toy", 15, 2, null);
        this.repository.Add(reward);

        // change state using domain method
        reward.DecreaseStock(); // now AvailableQuantity == 1

        // call Update with same instance (repository will replace the stored item)
        this.repository.Update(reward);

        var found = this.repository.GetById(reward.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(1, found!.AvailableQuantity);
    }

    [TestMethod]
    public void Update_ShouldDoNothing_WhenRewardNotPresent()
    {
        var existing = new Reward("Exist", "exists", 10, 1, null);
        this.repository.Add(existing);

        var notAdded = new Reward("Other", "not added", 12, 4, null);

        // attempt to update an item that is not in repository
        this.repository.Update(notAdded);

        var all = this.repository.GetAll().ToList();
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual(existing.Id, all[0].Id);
    }
}
