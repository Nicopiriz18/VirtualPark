// <copyright file="ScoreHistoryEntryDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Scoring.Responses;

public class ScoreHistoryEntryDto
{
    public required Guid Id { get; set; }

    public required DateTime Date { get; set; }

    public required int PointsAwarded { get; set; }

    public required string Origin { get; set; }

    public required string StrategyUsed { get; set; }

    public string? AttractionName { get; set; }

    public required string Description { get; set; }
}
