// <copyright file="RewardRedemptionControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class RewardRedemptionControllerTests
{
    private Mock<IRewardRedemptionRepository> mockRepo = null!;
    private RewardRedemptionController controller = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockRepo = new Mock<IRewardRedemptionRepository>();
        this.controller = new RewardRedemptionController(this.mockRepo.Object);
    }

    [TestMethod]
    public void GetByVisitor_ReturnsList()
    {
        var visitorId = Guid.NewGuid();
        var redemptions = new[]
        {
            new RewardRedemption(visitorId, Guid.NewGuid(), 50, DateTime.UtcNow),
        };

        this.mockRepo
            .Setup(r => r.GetByVisitor(visitorId))
            .Returns(redemptions);

        var result = this.controller.GetByVisitor(visitorId);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var payload = okResult!.Value as IEnumerable<object>;
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.Count());
    }
}
