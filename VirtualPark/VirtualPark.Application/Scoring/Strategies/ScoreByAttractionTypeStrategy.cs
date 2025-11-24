// <copyright file="ScoreByAttractionTypeStrategy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Scoring.Strategies;

public class ScoreByAttractionTypeStrategy : IScoringStrategy
{
    public string Name => "ScoreByAttractionType";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        return attraction.Type.ToLower() switch
        {
            "rollercoaster" => 100,
            "hauntedhouse" => 80,
            "waterride" => 70,
            "ferriswheel" => 60,
            "carousel" => 50,
            "bumpercars" => 40,
            _ => 30, // Default for unknown types
        };
    }
}
