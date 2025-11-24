// <copyright file="IScoreLogRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IScoreLogRepository
{
    ScoreLog Add(ScoreLog scoreLog);

    IEnumerable<ScoreLog> GetByVisitor(Guid visitorId);

    IEnumerable<ScoreLog> GetByDate(DateTime date);

    int TotalPointsByVisitor(Guid visitorId);
}
