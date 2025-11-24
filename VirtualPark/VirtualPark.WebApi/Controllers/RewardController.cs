// <copyright file="RewardController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Session;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Rewards;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RewardController(IRewardService service, ISessionService sessionService) : ControllerBase
{
    private readonly IRewardService service = service;
    private readonly ISessionService sessionService = sessionService;

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Create([FromBody] CreateRewardDto dto)
    {
        var reward = this.service.CreateReward(dto.Name, dto.Description, dto.CostInPoints, dto.AvailableQuantity, dto.RequiredLevel);
        return this.CreatedAtAction(nameof(this.Create), new { id = reward.Id }, reward);
    }

    [HttpPost("{rewardId}/redeem")]
    [AuthorizationFilter(RoleEnum.Visitor)]
    public IActionResult Redeem(Guid rewardId)
    {
        // Obtener el token del encabezado Authorization
        var token = this.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
        if (string.IsNullOrEmpty(token))
        {
            return this.Unauthorized();
        }

        // Buscar la sesión en la base de datos
        var user = this.sessionService.GetByToken(token);
        if (user == null)
        {
            return this.Unauthorized();
        }

        // El UserId de la sesión es el VisitorId
        var visitorId = user.Id;

        var redemption = this.service.Redeem(visitorId, rewardId);
        return this.Ok(new { Message = "Recompensa canjeada con éxito", redemption });
    }

    [HttpGet]
    public IActionResult GetAllRewards()
    {
        var rewards = this.service.GetAllRewards();
        return this.Ok(rewards);
    }
}
