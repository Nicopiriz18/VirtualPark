// <copyright file="SessionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class SessionRepository(ParkDbContext context) : ISessionRepository
{
    public Session Add(Session session)
    {
        context.Sessions.Add(session);
        context.SaveChanges();
        return session;
    }

    public Session? GetByToken(string token)
    {
        return context.Sessions.FirstOrDefault(s => s.Token == token);
    }

    public void DeleteByUserId(Guid userId)
    {
        var sessions = context.Sessions.Where(s => s.UserId == userId).ToList();
        context.Sessions.RemoveRange(sessions);
        context.SaveChanges();
    }
}
