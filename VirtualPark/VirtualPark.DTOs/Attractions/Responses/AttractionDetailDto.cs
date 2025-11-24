// <copyright file="AttractionDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Attractions.Responses;

public class AttractionDetailDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Type { get; set; }

    public required int MinAge { get; set; }

    public required int Capacity { get; set; }

    public required List<IncidenceDto> Incidences { get; set; }

    public int TotalIncidences { get; set; }

    public int ActiveIncidences { get; set; }
}
