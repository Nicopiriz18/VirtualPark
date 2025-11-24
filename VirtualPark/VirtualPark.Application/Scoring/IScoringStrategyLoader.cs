// <copyright file="IScoringStrategyLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Scoring;

public interface IScoringStrategyLoader
{
    Dictionary<string, IScoringStrategy> GetStrategies();
}
