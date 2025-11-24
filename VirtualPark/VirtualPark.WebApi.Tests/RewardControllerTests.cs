// <copyright file="RewardControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Session;
using VirtualPark.Domain;
using VirtualPark.DTOs.Rewards;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class RewardControllerTests
{
    private Mock<IRewardService> rewardServiceMock = null!;
    private Mock<ISessionService> sessionServiceMock = null!;
    private RewardController controller = null!;

    [TestInitialize]
    public void Setup()
    {
        this.rewardServiceMock = new Mock<IRewardService>();
        this.sessionServiceMock = new Mock<ISessionService>();
        this.controller = new RewardController(this.rewardServiceMock.Object, this.sessionServiceMock.Object);
    }

    [TestMethod]
    public void Create_ReturnsOk_WhenRewardIsCreated()
    {
        // Arrange
        var dto = new CreateRewardDto
        {
            Name = "Popcorn",
            Description = "Tasty popcorn",
            CostInPoints = 50,
            AvailableQuantity = 10,
            RequiredLevel = Domain.Enums.MembershipLevel.Standard,
        };

        var reward = new Reward(dto.Name, dto.Description, dto.CostInPoints, dto.AvailableQuantity, dto.RequiredLevel);
        this.rewardServiceMock.Setup(s => s.CreateReward(
            dto.Name, dto.Description, dto.CostInPoints, dto.AvailableQuantity, dto.RequiredLevel))
            .Returns(reward);

        // Act
        var result = this.controller.Create(dto) as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(reward, result.Value);
    }

    [TestMethod]
    public void Redeem_ReturnsOk_WhenRedemptionIsSuccessful()
    {
        // Arrange
        var rewardId = Guid.NewGuid();
        var token = "test-token";

        // create a User (session service returns a User)
        var user = new Domain.User("Test", "User", "test@example.com", "pwd", Domain.Enums.RoleEnum.Visitor);
        var visitorId = user.Id;

        var redemption = new RewardRedemption(visitorId, rewardId, 50, DateTime.UtcNow);
        this.sessionServiceMock.Setup(s => s.GetByToken(token)).Returns(user);
        this.rewardServiceMock.Setup(s => s.Redeem(visitorId, rewardId)).Returns(redemption);

        this.controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
        };
        this.controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        // Act
        var result = this.controller.Redeem(rewardId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void Redeem_ReturnsUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        this.controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
        };

        // Act
        var result = this.controller.Redeem(Guid.NewGuid()) as UnauthorizedResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void Redeem_ReturnsUnauthorized_WhenSessionIsInvalid()
    {
        // Arrange
        var token = "invalid-token";
        this.sessionServiceMock.Setup(s => s.GetByToken(token)).Returns((Domain.User?)null);

        this.controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
        };
        this.controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        // Act
        var result = this.controller.Redeem(Guid.NewGuid()) as UnauthorizedResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void GetAllRewards_ReturnsOkWithRewards()
    {
        // Arrange
        var rewards = new[]
        {
            new Reward("Sundae", "Ice cream", 30, 5),
            new Reward("T-shirt", "Park shirt", 100, 2, Domain.Enums.MembershipLevel.Premium),
        };

        this.rewardServiceMock.Setup(s => s.GetAllRewards()).Returns(rewards);

        // Act
        var result = this.controller.GetAllRewards() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(rewards, result.Value);
    }
}
