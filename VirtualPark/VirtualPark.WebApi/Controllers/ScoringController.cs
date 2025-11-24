// <copyright file="ScoringController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Scoring;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Scoring.Requests;
using VirtualPark.DTOs.Scoring.Responses;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScoringController(IScoringService scoringService, IStrategyPluginStore pluginStore) : ControllerBase
{
    private readonly IScoringService scoringService = scoringService;
    private readonly IStrategyPluginStore pluginStore = pluginStore;

    [HttpGet("strategies")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult GetStrategies()
    {
        var strategies = this.scoringService.GetAvailableStrategies().ToList();
        var active = this.scoringService.GetActiveStrategy();

        return this.Ok(new
        {
            AvailableStrategies = strategies,
            ActiveStrategy = active,
        });
    }

    [HttpGet("strategies/active")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult GetActiveStrategy()
    {
        var active = this.scoringService.GetActiveStrategy();

        return this.Ok(new
        {
            ActiveStrategy = active,
        });
    }

    [HttpPut("strategies/active")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult SetActiveStrategy([FromBody] SetActiveScoringStrategyRequestDto request)
    {
        try
        {
            this.scoringService.SetActiveStrategy(request.StrategyName);
            return this.Ok(new
            {
                Message = $"Strategy '{request.StrategyName}' activated successfully",
            });
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(new
            {
                Error = ex.Message,
            });
        }
    }

    [HttpPost("strategies/plugins")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadStrategyPlugin([FromForm] UploadStrategyPluginRequestDto request)
    {
        if (request?.Plugin is null)
        {
            return this.BadRequest(new
            {
                Error = "Se requiere un archivo DLL.",
            });
        }

        try
        {
            await this.pluginStore.StoreAsync(request.Plugin);
            var uploadedFileName = Path.GetFileName(request.Plugin.FileName ?? string.Empty);
            return this.Ok(new
            {
                Message = $"Plugin '{uploadedFileName}' cargado correctamente.",
            });
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(new
            {
                Error = ex.Message,
            });
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Error = "No se pudo guardar el plugin. Intenta nuevamente mas tarde.",
            });
        }
    }

    [HttpGet("ranking/daily")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult GetDailyRanking([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.Today;
        var ranking = this.scoringService.GetDailyRanking(targetDate, 10);

        var rankingDto = ranking.Select(r => new VisitorScoreDto
        {
            VisitorId = r.VisitorId,
            VisitorName = r.VisitorName,
            TotalPoints = r.TotalPoints,
            Position = r.Position,
            AccessCount = 0,
            LastActivity = null,
        }).ToList();

        return this.Ok(new
        {
            Date = targetDate.ToString("yyyy-MM-dd"),
            Ranking = rankingDto,
        });
    }
}
