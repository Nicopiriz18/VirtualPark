// <copyright file="TicketsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Tickets;
using VirtualPark.DTOs.Tickets.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizationFilter]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    private readonly ITicketService ticketService = ticketService;

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Visitor)]
    public IActionResult PurchaseTicket([FromBody] PurchaseTicketRequestDto request)
    {
        try
        {
            Ticket ticket;

            if (request.Type == TicketType.SpecialEvent)
            {
                if (request.SpecialEventId == null || request.SpecialEventId == Guid.Empty)
                {
                    return this.BadRequest(new { message = "SpecialEventId is required for special event tickets" });
                }

                ticket = this.ticketService.PurchaseSpecialEventTicket(
                    request.VisitorId,
                    request.VisitDate,
                    request.SpecialEventId.Value);
            }
            else
            {
                ticket = this.ticketService.PurchaseTicket(
                    request.VisitorId,
                    request.VisitDate,
                    request.Type);
            }

            var response = ticket.ToDto();

            return this.CreatedAtAction(nameof(this.PurchaseTicket), new { id = ticket.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return this.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { message = ex.Message });
        }
    }
}
