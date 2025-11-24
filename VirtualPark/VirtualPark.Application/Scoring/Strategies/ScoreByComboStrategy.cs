// <copyright file="ScoreByComboStrategy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Scoring.Strategies;
public class ScoreByComboStrategy : IScoringStrategy
{
    private const int BASEPOINTS = 50;
    private const int BONUSPERDIFFERENTATTRACTION = 25;
    private const int COMBOWINDOWMINUTES = 30;

    public string Name => "ScoreByCombo";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        if (!todayAccesses.Any())
        {
            return BASEPOINTS + BONUSPERDIFFERENTATTRACTION;
        }

        var currentAccessTime = todayAccesses.Max(a => a.EntryTime);

        var windowStart = currentAccessTime.AddMinutes(-COMBOWINDOWMINUTES);
        var accessesInWindow = todayAccesses
            .Where(a => a.EntryTime >= windowStart && a.EntryTime <= currentAccessTime)
            .ToList();

        var uniqueAttractions = accessesInWindow
            .Select(a => a.AttractionId)
            .Distinct()
            .Count();

        return BASEPOINTS + (uniqueAttractions * BONUSPERDIFFERENTATTRACTION);
    }
}
