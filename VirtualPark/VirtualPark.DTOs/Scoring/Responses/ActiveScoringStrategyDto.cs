// <copyright file="ActiveScoringStrategyDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Scoring.Responses;

public class ActiveScoringStrategyDto
{
    public required int Id { get; set; }

    public required string StrategyName { get; set; }

    public string? Description { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public List<string>? AvailableStrategies { get; set; }
}
