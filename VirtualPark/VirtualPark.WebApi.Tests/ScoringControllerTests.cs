// <copyright file="ScoringControllerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtualPark.Application.Scoring;
using VirtualPark.DTOs.Scoring.Requests;
using VirtualPark.DTOs.Scoring.Responses;
using VirtualPark.WebApi.Controllers;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class ScoringControllerTests
{
    [TestMethod]
    public void GetStrategies_ReturnsOk_WithAvailableAndActiveStrategies()
    {
        var strategies = new List<string> { "ScoreByAttractionType", "ScoreByCombo", "ScoreByEventMultiplier" };
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetAvailableStrategies()).Returns(strategies);
        mockService.Setup(s => s.GetActiveStrategy()).Returns("ScoreByAttractionType");
        var controller = CreateController(mockService);

        var result = controller.GetStrategies() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var value = result.Value;
        Assert.IsNotNull(value);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetActiveStrategy_ReturnsOk_WithActiveStrategyName()
    {
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetActiveStrategy()).Returns("ScoreByCombo");
        var controller = CreateController(mockService);

        var result = controller.GetActiveStrategy() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void SetActiveStrategy_ReturnsOk_WhenStrategyIsValid()
    {
        var request = new SetActiveScoringStrategyRequestDto { StrategyName = "ScoreByCombo" };
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.SetActiveStrategy("ScoreByCombo"));
        var controller = CreateController(mockService);

        var result = controller.SetActiveStrategy(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void SetActiveStrategy_ReturnsBadRequest_WhenStrategyIsInvalid()
    {
        var request = new SetActiveScoringStrategyRequestDto { StrategyName = "InvalidStrategy" };
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.SetActiveStrategy("InvalidStrategy"))
            .Throws(new ArgumentException("Strategy 'InvalidStrategy' not found"));
        var controller = CreateController(mockService);

        var result = controller.SetActiveStrategy(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        mockService.VerifyAll();
    }

    [TestMethod]
    public void GetDailyRanking_ReturnsOk_WithRankingForToday()
    {
        var ranking = new List<DailyRankingEntry>
        {
            new DailyRankingEntry { VisitorId = Guid.NewGuid(), VisitorName = "John Doe", TotalPoints = 180, Position = 1 },
            new DailyRankingEntry { VisitorId = Guid.NewGuid(), VisitorName = "Jane Smith", TotalPoints = 150, Position = 2 },
        };
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetDailyRanking(It.IsAny<DateTime>(), 10)).Returns(ranking);
        var controller = CreateController(mockService);

        var result = controller.GetDailyRanking(null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        // Verificar que el resultado tiene la estructura correcta con DTOs
        var value = result.Value;
        Assert.IsNotNull(value);
        var bodyType = value.GetType();
        var rankingProperty = bodyType.GetProperty("Ranking");
        Assert.IsNotNull(rankingProperty);
        var rankingDtos = rankingProperty.GetValue(value) as IEnumerable<VisitorScoreDto>;
        Assert.IsNotNull(rankingDtos);
        Assert.AreEqual(2, rankingDtos.Count());

        mockService.Verify(s => s.GetDailyRanking(It.IsAny<DateTime>(), 10), Times.Once);
    }

    [TestMethod]
    public void GetDailyRanking_ReturnsOk_WithRankingForSpecificDate()
    {
        var targetDate = new DateTime(2025, 10, 2);
        var ranking = new List<DailyRankingEntry>
        {
            new DailyRankingEntry { VisitorId = Guid.NewGuid(), VisitorName = "John Doe", TotalPoints = 180, Position = 1 },
        };
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetDailyRanking(targetDate, 10)).Returns(ranking);
        var controller = CreateController(mockService);

        var result = controller.GetDailyRanking(targetDate) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        mockService.Verify(s => s.GetDailyRanking(targetDate, 10), Times.Once);
    }

    [TestMethod]
    public void GetDailyRanking_ReturnsOk_WithEmptyRanking_WhenNoScoreLogs()
    {
        var emptyRanking = new List<DailyRankingEntry>();
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        mockService.Setup(s => s.GetDailyRanking(It.IsAny<DateTime>(), 10)).Returns(emptyRanking);
        var controller = CreateController(mockService);

        var result = controller.GetDailyRanking(null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var value = result.Value;
        Assert.IsNotNull(value);
    }

    [TestMethod]
    public async Task UploadStrategyPlugin_ReturnsOk_WhenFileSaved()
    {
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        var mockStore = new Mock<IStrategyPluginStore>(MockBehavior.Strict);
        var file = CreateFormFile("CustomStrategy.dll");
        var request = new UploadStrategyPluginRequestDto { Plugin = file };

        mockStore
            .Setup(s => s.StoreAsync(It.Is<IFormFile>(f => f.FileName == file.FileName), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var controller = CreateController(mockService, mockStore);

        var result = await controller.UploadStrategyPlugin(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        mockStore.Verify(s => s.StoreAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task UploadStrategyPlugin_ReturnsBadRequest_WhenStoreThrowsArgumentException()
    {
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        var mockStore = new Mock<IStrategyPluginStore>(MockBehavior.Strict);
        var file = CreateFormFile("CustomStrategy.dll");
        var request = new UploadStrategyPluginRequestDto { Plugin = file };

        mockStore
            .Setup(s => s.StoreAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid plugin"));

        var controller = CreateController(mockService, mockStore);

        var result = await controller.UploadStrategyPlugin(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        mockStore.Verify(s => s.StoreAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task UploadStrategyPlugin_ReturnsBadRequest_WhenFileIsMissing()
    {
        var mockService = new Mock<IScoringService>(MockBehavior.Strict);
        var mockStore = new Mock<IStrategyPluginStore>(MockBehavior.Strict);

        var controller = CreateController(mockService, mockStore);

        var result = await controller.UploadStrategyPlugin(null) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        mockStore.Verify(s => s.StoreAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static ScoringController CreateController(Mock<IScoringService> mockService, Mock<IStrategyPluginStore>? pluginStore = null)
    {
        var store = pluginStore ?? new Mock<IStrategyPluginStore>(MockBehavior.Strict);
        return new ScoringController(mockService.Object, store.Object);
    }

    private static IFormFile CreateFormFile(string fileName)
    {
        var bytes = Encoding.UTF8.GetBytes("dummy plugin");
        var stream = new MemoryStream(bytes);
        stream.Position = 0;
        return new FormFile(stream, 0, bytes.Length, "plugin", fileName);
    }
}
