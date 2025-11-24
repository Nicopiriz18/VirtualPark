// <copyright file="ScoreByEventMultiplierStrategy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Scoring.Strategies;

public class ScoreByEventMultiplierStrategy : IScoringStrategy
{
    private const int BASEPOINTS = 50;
    private const decimal EVENTMULTIPLIER = 2.0m;

    public string Name => "ScoreByEventMultiplier";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        if (activeEvent == null)
        {
            return BASEPOINTS;
        }

        var isAttractionInEvent = activeEvent.Attractions
            .Any(a => a.Id == attraction.Id);

        if (isAttractionInEvent)
        {
            return (int)(BASEPOINTS * EVENTMULTIPLIER);
        }

        return BASEPOINTS;
    }
}
