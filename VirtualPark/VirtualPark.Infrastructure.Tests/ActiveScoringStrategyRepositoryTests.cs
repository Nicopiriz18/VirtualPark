// <copyright file="ActiveScoringStrategyRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class ActiveScoringStrategyRepositoryTests
{
    private ActiveScoringStrategyRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new ActiveScoringStrategyRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void SetActive_ShouldCreateRow_WhenNone()
    {
        this.repository.SetActive("ScoreByCombo");

        var stored = this.context.ActiveScoringStrategies.FirstOrDefault();
        Assert.IsNotNull(stored);
        Assert.AreEqual(1, stored!.Id);
        Assert.AreEqual("ScoreByCombo", stored.StrategyName);
    }

    [TestMethod]
    public void SetActive_ShouldUpdateExistingRow()
    {
        // seed initial
        this.context.ActiveScoringStrategies.Add(new ActiveScoringStrategy { Id = 1, StrategyName = "ScoreByAttractionType" });
        this.context.SaveChanges();

        this.repository.SetActive("ScoreByEventMultiplier");

        var stored = this.context.ActiveScoringStrategies.First(a => a.Id == 1);
        Assert.AreEqual("ScoreByEventMultiplier", stored.StrategyName);
    }

    [TestMethod]
    public void GetActive_ShouldReturnNull_WhenMissing()
    {
        var active = this.repository.GetActive();
        Assert.IsNull(active);
    }

    [TestMethod]
    public void GetActive_ShouldReturnFirstRow_WhenPresent()
    {
        var dto = new ActiveScoringStrategy { Id = 1, StrategyName = "ScoreByCombo" };
        this.context.ActiveScoringStrategies.Add(dto);
        this.context.SaveChanges();

        var active = this.repository.GetActive();
        Assert.IsNotNull(active);
        Assert.AreEqual("ScoreByCombo", active!.StrategyName);
    }
}
