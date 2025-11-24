// <copyright file="ClockController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Clock;
using VirtualPark.DTOs.Common;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClockController(IClockService service) : ControllerBase
{
    private readonly IClockService service = service;

    [HttpGet]
    public IActionResult Get()
    {
        var now = this.service.GetNow();
        var response = now.ToClockResponseDto();
        return this.Ok(response);
    }

    [HttpPut]
    public IActionResult Put([FromBody] ClockRequestDto request)
    {
        if (!DateTime.TryParse(request.Value, out var newNow))
        {
            return this.BadRequest("Invalid format. Expected YYYY-MM-DDTHH:MM");
        }

        this.service.SetNow(newNow);
        return this.NoContent();
    }
}
