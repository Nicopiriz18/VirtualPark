// <copyright file="AttractionAccessController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.AttractionAccess;
using VirtualPark.DTOs.AttractionAccess.Requests;
using VirtualPark.DTOs.Attractions;
using VirtualPark.DTOs.Attractions.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttractionAccessController(IAttractionAccessService attractionAccessService, IAttractionIncidenceService attractionService, IClockService clockService) : ControllerBase
{
    private readonly IAttractionAccessService service = attractionAccessService;
    private readonly IAttractionIncidenceService attractionService = attractionService;
    private readonly IClockService clockService = clockService;

    [HttpPost("{attractionId}/access")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult RegisterAccess(Guid attractionId, [FromBody] RegisterAccessRequestDto request)
    {
        // Validación del contrato de entrada
        if (request.EntryMethod == EntryMethod.QR && request.QrCode is null)
        {
            return this.BadRequest(new { Error = "QrCode es requerido cuando EntryMethod=QR." });
        }

        if (request.EntryMethod == EntryMethod.NFC && request.VisitorId is null)
        {
            return this.BadRequest(new { Error = "VisitorId es requerido cuando EntryMethod=NFC." });
        }

        // Call the appropriate service method based on entry method
        if (request.EntryMethod == EntryMethod.QR)
        {
            this.service.RegisterAccessByQrCode(attractionId, request.QrCode!.Value, request.EntryMethod);
        }
        else
        {
            this.service.RegisterAccessByVisitorId(attractionId, request.VisitorId!.Value, request.VisitDate, request.EntryMethod);
        }

        return this.Ok(new
        {
            Message = "Acceso registrado con éxito",
            AttractionId = attractionId,
            VisitorId = request.VisitorId,
            EntryMethod = request.EntryMethod.ToString(),
        });
    }

    [HttpPut("{attractionId}/exit")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult RegisterExit(Guid attractionId, [FromQuery] Guid visitorId)
    {
        DateTime currentTime = this.clockService.GetNow();
        this.service.RegisterExit(attractionId, visitorId, currentTime);
        return this.Ok(new
        {
            Message = "Salida registrada con éxito",
            AttractionId = attractionId,
            VisitorId = visitorId,
            ExitTime = currentTime,
        });
    }

    [HttpGet("capacity")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult GetCapacity(Guid attractionId)
    {
        var (aforo, restante) = this.service.GetAforo(attractionId);
        return this.Ok(new
        {
            AttractionId = attractionId,
            CurrentOccupancy = aforo,
            RemainingCapacity = restante,
        });
    }

    [HttpPost("{attractionId}/incidents")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult CreateIncident(Guid attractionId, [FromBody] CreateIncidenceRequestDto request)
    {
        try
        {
            var incident = this.attractionService.AddIncidence(attractionId, request.Title, request.Description, request.Status, request.Date);
            return this.CreatedAtAction(nameof(this.GetIncidents), new { attractionId, incidentId = incident.Id }, incident.ToDto());
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{attractionId}/incidents")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult UpdateIncident(Guid attractionId, [FromQuery] Guid incidentId, [FromQuery] string action)
    {
        try
        {
            if (action == "close")
            {
                this.attractionService.CloseIncidence(attractionId, incidentId);
                return this.Ok(new { Message = "Incidencia cerrada con éxito" });
            }
            else if (action == "reopen")
            {
                this.attractionService.ReopenIncidence(attractionId, incidentId);
                return this.Ok(new { Message = "Incidencia reabierta con éxito" });
            }
            else
            {
                return this.BadRequest(new { Message = "Acción no válida. Use 'close' o 'reopen'." });
            }
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("{attractionId}/incidents")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult GetIncidents(Guid attractionId, [FromQuery] Guid? incidentId)
    {
        try
        {
            var incidents = this.attractionService.GetIncidences(attractionId);

            if (incidentId.HasValue)
            {
                var incident = incidents.FirstOrDefault(i => i.Id == incidentId.Value);

                if (incident == null)
                {
                    return this.NotFound(new { Message = "Incidencia no encontrada" });
                }

                return this.Ok(incident.ToDto());
            }

            return this.Ok(incidents.ToDto());
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("{attractionId}/status")]
    [AuthorizationFilter(RoleEnum.Operator)]
    public IActionResult GetAttractionStatus(Guid attractionId)
    {
        try
        {
            var activeIncidents = this.attractionService.GetActiveIncidences(attractionId);
            var isOutOfService = activeIncidents.Any();
            return this.Ok(new { AttractionId = attractionId, IsOutOfService = isOutOfService });
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }
}
