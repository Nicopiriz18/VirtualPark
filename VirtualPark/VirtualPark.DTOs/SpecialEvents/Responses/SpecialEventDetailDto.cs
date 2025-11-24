// <copyright file="SpecialEventDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.DTOs.Attractions.Responses;

namespace VirtualPark.DTOs.SpecialEvents.Responses;

public class SpecialEventDetailDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTime Date { get; set; }

    public required int MaxCapacity { get; set; }

    public required decimal AdditionalCost { get; set; }

    public required List<AttractionDto> Attractions { get; set; }

    public int TotalAttractions { get; set; }
}
