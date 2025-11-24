// <copyright file="VisitorDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Tickets.Responses;

namespace VirtualPark.DTOs.Visitors.Responses;

public class VisitorDetailDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string Email { get; set; }

    public required DateTime BirthDate { get; set; }

    public required MembershipLevel MembershipLevel { get; set; }

    public required Guid NfcId { get; set; }

    public required List<TicketDto> Tickets { get; set; }

    public int TotalTickets { get; set; }
}
