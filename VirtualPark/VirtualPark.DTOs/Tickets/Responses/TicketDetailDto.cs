// <copyright file="TicketDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Tickets.Responses;

public class TicketDetailDto
{
    public required Guid Id { get; set; }

    public required Guid VisitorId { get; set; }

    public required string VisitorName { get; set; }

    public required DateTime VisitDate { get; set; }

    public required TicketType Type { get; set; }

    public required Guid QrCode { get; set; }

    public Guid? SpecialEventId { get; set; }

    public string? SpecialEventName { get; set; }

    public decimal? SpecialEventCost { get; set; }
}
