// <copyright file="IncidenceDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Attractions.Responses;

public class IncidenceDto
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required bool Status { get; set; }

    public required DateTime Date { get; set; }

    public required Guid AttractionId { get; set; }

    public string? AttractionName { get; set; }
}
