// <copyright file="IScoringStrategy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Scoring;

public interface IScoringStrategy
{
    string Name { get; }

    int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent);
}
