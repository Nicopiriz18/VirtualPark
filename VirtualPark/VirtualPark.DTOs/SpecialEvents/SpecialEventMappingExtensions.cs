// <copyright file="SpecialEventMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Attractions;
using VirtualPark.DTOs.SpecialEvents.Requests;
using VirtualPark.DTOs.SpecialEvents.Responses;

namespace VirtualPark.DTOs.SpecialEvents;

public static class SpecialEventMappingExtensions
{
    public static SpecialEventDto ToDto(this SpecialEvent specialEvent)
    {
        return new SpecialEventDto
        {
            Id = specialEvent.Id,
            Name = specialEvent.Name,
            Date = specialEvent.Date,
            MaxCapacity = specialEvent.MaxCapacity,
            AdditionalCost = specialEvent.AdditionalCost,
        };
    }

    public static SpecialEventDetailDto ToDetailDto(this SpecialEvent specialEvent)
    {
        return new SpecialEventDetailDto
        {
            Id = specialEvent.Id,
            Name = specialEvent.Name,
            Date = specialEvent.Date,
            MaxCapacity = specialEvent.MaxCapacity,
            AdditionalCost = specialEvent.AdditionalCost,
            Attractions = specialEvent.Attractions.ToDto(),
            TotalAttractions = specialEvent.Attractions.Count,
        };
    }

    public static SpecialEventCapacityDto ToCapacityDto(this SpecialEvent specialEvent, int currentTicketsSold, int requestedTickets)
    {
        var availableCapacity = specialEvent.MaxCapacity - currentTicketsSold;
        return new SpecialEventCapacityDto
        {
            EventId = specialEvent.Id,
            EventName = specialEvent.Name,
            MaxCapacity = specialEvent.MaxCapacity,
            CurrentTicketsSold = currentTicketsSold,
            AvailableCapacity = availableCapacity,
            RequestedTickets = requestedTickets,
            HasCapacity = availableCapacity >= requestedTickets,
        };
    }

    public static SpecialEvent ToDomain(this CreateSpecialEventRequestDto dto)
    {
        return new SpecialEvent(
            dto.Name,
            dto.Date,
            dto.MaxCapacity,
            dto.AdditionalCost);
    }

    public static List<SpecialEventDto> ToDto(this IEnumerable<SpecialEvent> specialEvents)
    {
        return specialEvents.Select(se => se.ToDto()).ToList();
    }

    public static List<SpecialEventDetailDto> ToDetailDto(this IEnumerable<SpecialEvent> specialEvents)
    {
        return specialEvents.Select(se => se.ToDetailDto()).ToList();
    }
}
