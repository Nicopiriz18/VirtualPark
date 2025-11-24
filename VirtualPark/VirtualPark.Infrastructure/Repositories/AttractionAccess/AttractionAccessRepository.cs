// <copyright file="AttractionAccessRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class AttractionAccessRepository(ParkDbContext context) : IAttractionAccessRepository
{
    private readonly ParkDbContext context = context;

    // Persiste y devuelve la entidad creada
    public AttractionAccess Add(AttractionAccess entity)
    {
        this.context.Set<AttractionAccess>().Add(entity); // compatible aunque DbSet se llame AttractionAccesses
        this.context.SaveChanges();
        return entity;
    }

    public IEnumerable<AttractionAccess> GetAll() =>
        this.context.Set<AttractionAccess>().AsNoTracking().ToList();

    public AttractionAccess? GetOpenAccess(Guid attractionId, Guid visitorId)
    {
        return this.context.AttractionAccesses
            .FirstOrDefault(a => a.AttractionId == attractionId &&
                                 a.VisitorId == visitorId &&
                                 a.ExitTime == null);
    }

    public void Update(AttractionAccess entity)
    {
        this.context.Set<AttractionAccess>().Update(entity);
        this.context.SaveChanges();
    }

    public int CountOpenAccesses(Guid attractionId)
    {
        return this.context.AttractionAccesses
            .Count(a => a.AttractionId == attractionId && a.ExitTime == null);
    }

    public AttractionAccess? GetOpenAccessByTicket(Guid ticketId)
    {
        return this.context.AttractionAccesses
            .FirstOrDefault(a => a.TicketId == ticketId && a.ExitTime == null);
    }

    public IEnumerable<AttractionAccess> GetAccessesBetweenDates(DateTime startDate, DateTime endDate)
    {
        return this.context.AttractionAccesses
            .Where(a => a.EntryTime >= startDate && a.EntryTime <= endDate)
            .ToList();
    }

    public IEnumerable<AttractionAccess> GetAccessesByVisitorAndDate(Guid visitorId, DateTime date)
    {
        return this.context.AttractionAccesses
            .AsNoTracking()
            .Where(a => a.VisitorId == visitorId && a.EntryTime.Date == date.Date)
            .OrderBy(a => a.EntryTime)
            .ToList();
    }
}
