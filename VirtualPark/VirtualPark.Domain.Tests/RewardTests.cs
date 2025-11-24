// <copyright file="RewardTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace Domain.Tests;

[TestClass]
public class RewardTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        var reward = new Reward("Popcorn", "Delicious popcorn", 100, 5, MembershipLevel.Premium);

        Assert.AreNotEqual(Guid.Empty, reward.Id);
        Assert.AreEqual("Popcorn", reward.Name);
        Assert.AreEqual("Delicious popcorn", reward.Description);
        Assert.AreEqual(100, reward.CostInPoints);
        Assert.AreEqual(5, reward.AvailableQuantity);
        Assert.AreEqual(MembershipLevel.Premium, reward.RequiredLevel);
    }

    [TestMethod]
    public void Constructor_AllowsNullRequiredLevel()
    {
        var reward = new Reward("Ticket", "Park entry", 50, 10, null);

        Assert.IsNull(reward.RequiredLevel);
        Assert.AreEqual("Ticket", reward.Name);
        Assert.AreEqual(50, reward.CostInPoints);
        Assert.AreEqual(10, reward.AvailableQuantity);
    }

    [TestMethod]
    public void MultipleInstances_HaveDifferentIds()
    {
        var r1 = new Reward("A", "first", 1, 1, null);
        var r2 = new Reward("B", "second", 2, 2, null);

        Assert.AreNotEqual(r1.Id, r2.Id);
    }

    [TestMethod]
    public void Constructor_Throws_WhenNameIsNullOrWhitespace_Null()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward(null!, "desc", 10, 1, null));
        Assert.AreEqual("Name is required", ex.Message);
    }

    [TestMethod]
    public void Constructor_Throws_WhenNameIsNullOrWhitespace_Empty()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward(string.Empty, "desc", 10, 1, null));
        Assert.AreEqual("Name is required", ex.Message);
    }

    [TestMethod]
    public void Constructor_Throws_WhenNameIsNullOrWhitespace_Whitespace()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward("   ", "desc", 10, 1, null));
        Assert.AreEqual("Name is required", ex.Message);
    }

    [TestMethod]
    public void Constructor_Throws_WhenCostIsNotPositive_Zero()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward("X", "desc", 0, 1, null));
        Assert.AreEqual("Cost must be positive", ex.Message);
    }

    [TestMethod]
    public void Constructor_Throws_WhenCostIsNotPositive_Negative()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward("X", "desc", -5, 1, null));
        Assert.AreEqual("Cost must be positive", ex.Message);
    }

    [TestMethod]
    public void Constructor_Throws_WhenQuantityNegative()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            new Reward("X", "desc", 10, -1, null));
        Assert.AreEqual("Quantity must be >= 0", ex.Message);
    }

    [TestMethod]
    public void DecreaseStock_ReducesAvailableQuantity()
    {
        var reward = new Reward("Candy", "Sweet", 10, 3, null);
        reward.DecreaseStock();
        Assert.AreEqual(2, reward.AvailableQuantity);
    }

    [TestMethod]
    public void DecreaseStock_FromOne_ToZero()
    {
        var reward = new Reward("Toy", "Small toy", 5, 1, null);
        reward.DecreaseStock();
        Assert.AreEqual(0, reward.AvailableQuantity);
    }

    [TestMethod]
    public void DecreaseStock_Throws_WhenNoStock()
    {
        var reward = new Reward("Badge", "Collectible", 2, 0, null);
        var ex = Assert.ThrowsException<InvalidOperationException>(reward.DecreaseStock);
        Assert.AreEqual("Reward not available", ex.Message);
    }
}
