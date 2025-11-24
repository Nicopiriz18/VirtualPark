// <copyright file="SpecialEventDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.SpecialEvents.Responses;

public class SpecialEventDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTime Date { get; set; }

    public required int MaxCapacity { get; set; }

    public required decimal AdditionalCost { get; set; }
}
