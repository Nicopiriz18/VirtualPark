// <copyright file="RewardRedemptionController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RewardRedemptionController(IRewardRedemptionRepository repository) : ControllerBase
{
    private readonly IRewardRedemptionRepository repository = repository;

    [HttpGet("visitor/{visitorId}")]
    [AuthorizationFilter(RoleEnum.Visitor)]
    public IActionResult GetByVisitor(Guid visitorId)
    {
        var entries = this.repository.GetByVisitor(visitorId)
            .Select(rr => new
            {
                rr.Id,
                rr.RewardId,
                rr.VisitorId,
                rr.PointsSpent,
                rr.Date,
            })
            .ToList();

        return this.Ok(entries);
    }
}
