// <copyright file="TicketRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class TicketRepository(ParkDbContext context) : ITicketRepository
{
    private readonly ParkDbContext context = context;

    public IEnumerable<Ticket> GetAll() =>
        this.context.Tickets.AsNoTracking().ToList();

    public Ticket? GetById(Guid id) =>
        this.context.Tickets.Find(id);

    public Ticket? GetByQrCode(Guid qrCode) =>
        this.context.Tickets
            .Include(t => t.Visitor)
            .FirstOrDefault(t => t.QrCode == qrCode);

    public Ticket Add(Ticket ticket)
    {
        this.context.Tickets.Add(ticket);
        this.context.SaveChanges();
        return ticket;
    }

    public bool Update(Guid id, Ticket ticket)
    {
        var existing = this.context.Tickets.Find(id);
        if (existing is null)
        {
            return false;
        }

        this.context.Entry(existing).Property(nameof(Ticket.VisitDate)).CurrentValue = ticket.VisitDate;
        this.context.Entry(existing).Property(nameof(Ticket.Type)).CurrentValue = ticket.Type;
        this.context.Entry(existing).Property(nameof(Ticket.SpecialEventId)).CurrentValue = ticket.SpecialEventId;
        this.context.SaveChanges();
        return true;
    }

    public int CountTicketsByEventId(Guid eventId) =>
        this.context.Tickets.Count(t => t.SpecialEventId == eventId);
}
