// <copyright file="SessionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Session;

public class SessionService(IUserService userService, ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository sessionRepository = sessionRepository;
    private readonly IUserService userService = userService;

    public Domain.Session CreateSession(Guid userId, string token)
    {
        // Eliminar sesiones anteriores del usuario para evitar duplicados
        this.sessionRepository.DeleteByUserId(userId);

        var session = new Domain.Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
        };

        return this.sessionRepository.Add(session);
    }

    public bool IsSessionValid(string token, RoleEnum role)
    {
        var session = this.sessionRepository.GetByToken(token);
        if (session is null)
        {
            return false;
        }

        var user = this.userService.GetById(session.UserId);
        if (user is null)
        {
            return false;
        }

        return user.Roles.Contains(role);
    }

    public User? GetByToken(string token)
    {
        var session = this.sessionRepository.GetByToken(token);
        if (session is null)
        {
            throw new KeyNotFoundException("Session not found.");
        }

        var user = this.userService.GetById(session.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return user;
    }
}
