// <copyright file="TicketMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Tickets.Requests;
using VirtualPark.DTOs.Tickets.Responses;

namespace VirtualPark.DTOs.Tickets;

public static class TicketMappingExtensions
{
    public static TicketDto ToDto(this Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            VisitorId = ticket.VisitorId,
            VisitDate = ticket.VisitDate,
            Type = ticket.Type,
            QrCode = ticket.QrCode,
            SpecialEventId = ticket.SpecialEventId,
        };
    }

    public static TicketDetailDto ToDetailDto(this Ticket ticket, string visitorName, string? specialEventName = null, decimal? specialEventCost = null)
    {
        return new TicketDetailDto
        {
            Id = ticket.Id,
            VisitorId = ticket.VisitorId,
            VisitorName = visitorName,
            VisitDate = ticket.VisitDate,
            Type = ticket.Type,
            QrCode = ticket.QrCode,
            SpecialEventId = ticket.SpecialEventId,
            SpecialEventName = specialEventName,
            SpecialEventCost = specialEventCost,
        };
    }

    public static Ticket ToDomain(this PurchaseTicketRequestDto dto, Guid qrCode)
    {
        return new Ticket(
            dto.VisitDate,
            dto.Type,
            qrCode,
            dto.SpecialEventId);
    }

    public static List<TicketDto> ToDto(this IEnumerable<Ticket> tickets)
    {
        return tickets.Select(t => t.ToDto()).ToList();
    }
}
