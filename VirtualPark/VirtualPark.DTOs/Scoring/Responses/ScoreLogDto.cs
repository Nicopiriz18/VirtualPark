// <copyright file="ScoreLogDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Scoring.Responses;

public class ScoreLogDto
{
    public required Guid Id { get; set; }

    public required Guid VisitorId { get; set; }

    public required Guid? AttractionAccessId { get; set; }

    public required Guid? AttractionId { get; set; }

    public required int PointsAwarded { get; set; }

    public required string StrategyUsed { get; set; }

    public required DateTime AwardedAt { get; set; }

    public string? VisitorName { get; set; }

    public string? AttractionName { get; set; }
}
