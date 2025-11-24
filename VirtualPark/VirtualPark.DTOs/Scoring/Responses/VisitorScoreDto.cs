// <copyright file="VisitorScoreDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Scoring.Responses;

public class VisitorScoreDto
{
    public required Guid VisitorId { get; set; }

    public required string VisitorName { get; set; }

    public required int TotalPoints { get; set; }

    public required int Position { get; set; }

    public int AccessCount { get; set; }

    public DateTime? LastActivity { get; set; }
}
