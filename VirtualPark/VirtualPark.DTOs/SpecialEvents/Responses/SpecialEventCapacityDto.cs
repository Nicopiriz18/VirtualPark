// <copyright file="SpecialEventCapacityDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.SpecialEvents.Responses;

public class SpecialEventCapacityDto
{
    public required Guid EventId { get; set; }

    public required string EventName { get; set; }

    public required int MaxCapacity { get; set; }

    public required int CurrentTicketsSold { get; set; }

    public required int AvailableCapacity { get; set; }

    public required int RequestedTickets { get; set; }

    public required bool HasCapacity { get; set; }
}
