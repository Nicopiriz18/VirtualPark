// <copyright file="DailyRankingEntry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Scoring;

public class DailyRankingEntry
{
    public Guid VisitorId { get; set; }

    public string VisitorName { get; set; } = string.Empty;

    public int TotalPoints { get; set; }

    public int Position { get; set; }
}
