// <copyright file="ISessionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
    Session Add(Session session);

    Session? GetByToken(string token);

    void DeleteByUserId(Guid userId);
}
