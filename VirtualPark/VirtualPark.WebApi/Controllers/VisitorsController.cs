// <copyright file="VisitorsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Visitors;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Visitors.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizationFilter]
public class VisitorsController : ControllerBase
{
    private readonly IVisitorService visitorService;
    private readonly IScoreHistoryService scoreHistoryService;

    public VisitorsController(IVisitorService visitorService, IScoreHistoryService scoreHistoryService)
    {
        this.visitorService = visitorService;
        this.scoreHistoryService = scoreHistoryService;
    }

    /// <summary>
    /// Updates a visitor's profile information.
    /// </summary>
    /// <param name="id">The ID of the visitor to update.</param>
    /// <param name="request">The updated profile information.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The profile was successfully updated.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="404">The visitor was not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateProfile(Guid id, [FromBody] UpdateVisitorProfileRequestDto request)
    {
        try
        {
            this.visitorService.UpdateProfileWithPassword(id, request.Name, request.Surname, request.Email, request.Password);
            return this.NoContent();
        }
        catch (ArgumentException ex)
        {
            return this.NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{visitorId}/score-history")]
    [AuthorizationFilter(RoleEnum.Visitor)]
    public IActionResult GetScoreHistory(Guid visitorId)
    {
        try
        {
            var history = this.scoreHistoryService.GetVisitorHistory(visitorId);
            return this.Ok(history);
        }
        catch (KeyNotFoundException ex)
        {
            return this.NotFound(new { message = ex.Message });
        }
    }
}
