// <copyright file="IScoreHistoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.DTOs.Scoring.Responses;

namespace VirtualPark.Application.Scoring;

public interface IScoreHistoryService
{
    IEnumerable<ScoreHistoryEntryDto> GetVisitorHistory(Guid visitorId);
}
