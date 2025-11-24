// <copyright file="SpecialEventController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.SpecialEvent;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.SpecialEvents;
using VirtualPark.DTOs.SpecialEvents.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizationFilter]
public class SpecialEventController(ISpecialEventService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var events = service.GetAll();
        return this.Ok(events.ToDto());
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var specialEvent = service.GetById(id);
        if (specialEvent is null)
        {
            return this.NotFound();
        }

        return this.Ok(specialEvent.ToDetailDto());
    }

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Post([FromBody] CreateSpecialEventRequestDto request)
    {
        var createdEvent = service.Create(request.Name, request.Date, request.MaxCapacity, request.AdditionalCost);
        return this.CreatedAtAction(nameof(this.Get), new { id = createdEvent.Id }, createdEvent.ToDto());
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Delete(Guid id)
    {
        service.Delete(id);
        return this.NoContent();
    }

    [HttpPost("{eventId}/attractions")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult AddAttractionToEvent(Guid eventId, [FromBody] AddAttractionToEventRequestDto request)
    {
        try
        {
            service.AddAttraction(eventId, request.AttractionId);
            var updatedEvent = service.GetById(eventId);
            return this.Ok(updatedEvent?.ToDetailDto());
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{eventId}/attractions")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult RemoveAttractionFromEvent(Guid eventId, [FromQuery] Guid attractionId)
    {
        try
        {
            service.RemoveAttraction(eventId, attractionId);
            var updatedEvent = service.GetById(eventId);
            return this.Ok(updatedEvent?.ToDetailDto());
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpGet("{eventId}/capacity")]
    public IActionResult CheckCapacity(Guid eventId, [FromQuery] int ticketsRequested)
    {
        var specialEvent = service.GetById(eventId);
        if (specialEvent is null)
        {
            return this.NotFound();
        }

        var availableCapacity = service.GetAvailableCapacity(eventId);
        var currentTicketsSold = specialEvent.MaxCapacity - availableCapacity;
        var capacityDto = specialEvent.ToCapacityDto(currentTicketsSold, ticketsRequested);

        return this.Ok(capacityDto);
    }
}
