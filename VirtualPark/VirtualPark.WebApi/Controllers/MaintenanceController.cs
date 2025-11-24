// <copyright file="MaintenanceController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Maintenance;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Maintenance;
using VirtualPark.DTOs.Maintenance.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizationFilter]
public class MaintenanceController(IMaintenanceService service) : ControllerBase
{
    private readonly IMaintenanceService service = service;

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Post([FromBody] CreateMaintenanceRequestDto request)
    {
        var maintenance = this.service.ScheduleMaintenance(
            request.AttractionId,
            request.ScheduledDate,
            request.StartTime,
            request.EstimatedDuration,
            request.Description);

        return this.CreatedAtAction(
            nameof(this.Get),
            new { id = maintenance.Id },
            maintenance.ToDto());
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var maintenance = this.service.GetMaintenanceById(id);
        if (maintenance is null)
        {
            return this.NotFound();
        }

        return this.Ok(maintenance.ToDto());
    }

    [HttpGet("attraction/{attractionId}")]
    public IActionResult GetByAttraction(Guid attractionId)
    {
        var maintenances = this.service.GetMaintenancesByAttraction(attractionId);
        return this.Ok(maintenances.ToDto());
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Delete(Guid id)
    {
        try
        {
            this.service.CancelMaintenance(id);
            return this.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
    }
}
