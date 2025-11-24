// <copyright file="AttractionMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Attractions.Requests;
using VirtualPark.DTOs.Attractions.Responses;

namespace VirtualPark.DTOs.Attractions;

public static class AttractionMappingExtensions
{
    /// <summary>
    /// Converts an Attraction domain model to an AttractionDto.
    /// </summary>
    /// <param name="attraction">The attraction to convert.</param>
    /// <returns>An AttractionDto representing the attraction.</returns>
    public static AttractionDto ToDto(this Attraction attraction)
    {
        return new AttractionDto
        {
            Id = attraction.Id,
            Name = attraction.Name,
            Description = attraction.Description,
            Type = attraction.Type,
            MinAge = attraction.MinAge,
            Capacity = attraction.Capacity,
        };
    }

    public static AttractionDetailDto ToDetailDto(this Attraction attraction)
    {
        return new AttractionDetailDto
        {
            Id = attraction.Id,
            Name = attraction.Name,
            Description = attraction.Description,
            Type = attraction.Type,
            MinAge = attraction.MinAge,
            Capacity = attraction.Capacity,
            Incidences = attraction.Incidences.ToDto(),
            TotalIncidences = attraction.Incidences.Count,
            ActiveIncidences = attraction.Incidences.Count(i => i.Status),
        };
    }

    public static AttractionListDto ToListDto(this Attraction attraction)
    {
        return new AttractionListDto
        {
            Id = attraction.Id,
            Name = attraction.Name,
            Type = attraction.Type,
            MinAge = attraction.MinAge,
            Capacity = attraction.Capacity,
            HasActiveIncidences = attraction.Incidences.Any(i => i.Status),
        };
    }

    public static IncidenceDto ToDto(this Incidence incidence)
    {
        return new IncidenceDto
        {
            Id = incidence.Id,
            Title = incidence.Title,
            Description = incidence.Description,
            Status = incidence.Status,
            Date = incidence.Date,
            AttractionId = incidence.AttractionId,
            AttractionName = incidence.Attraction.Name,
        };
    }

    public static Attraction ToDomain(this CreateAttractionRequestDto dto)
    {
        return new Attraction(
            dto.Name,
            dto.Description,
            dto.Type,
            dto.MinAge,
            dto.Capacity);
    }

    public static Incidence ToIncidence(this CreateIncidenceRequestDto dto, Guid attractionId)
    {
        return new Incidence(
            dto.Title,
            dto.Description,
            dto.Status,
            dto.Date,
            attractionId);
    }

    public static List<AttractionDto> ToDto(this IEnumerable<Attraction> attractions)
    {
        return attractions.Select(a => a.ToDto()).ToList();
    }

    public static List<AttractionListDto> ToListDto(this IEnumerable<Attraction> attractions)
    {
        return attractions.Select(a => a.ToListDto()).ToList();
    }

    public static List<IncidenceDto> ToDto(this IEnumerable<Incidence> incidences)
    {
        return incidences.Select(i => i.ToDto()).ToList();
    }
}
