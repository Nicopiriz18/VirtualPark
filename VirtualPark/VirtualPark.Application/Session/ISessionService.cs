// <copyright file="ISessionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Session;

public interface ISessionService
{
    Domain.Session CreateSession(Guid userId, string token);

    User? GetByToken(string token);

    bool IsSessionValid(string token, RoleEnum role);
}
