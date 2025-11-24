// <copyright file="AttractionAccess.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain;
public class AttractionAccess
{
    private AttractionAccess()
    {
    }

    public Guid Id { get; set; }

    public Guid AttractionId { get; set; }

    public Guid VisitorId { get; set; }

    public Guid? TicketId { get; set; }

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public EntryMethod EntryMethod { get; set; }

    public bool IsClosed => this.ExitTime.HasValue;

    public AttractionAccess(
        Guid attractionId,
        Guid visitorId,
        Guid? ticketId,
        DateTime entryTime,
        DateTime? exitTime,
        EntryMethod method)
    {
        this.Id = Guid.NewGuid();
        this.AttractionId = attractionId;
        this.VisitorId = visitorId;
        this.TicketId = ticketId;
        this.EntryTime = entryTime;
        this.ExitTime = exitTime;
        this.EntryMethod = method;
    }

    public void MarkExit(DateTime exitTime)
    {
        if (this.ExitTime != null)
        {
            throw new InvalidOperationException("El egreso ya fue registrado.");
        }

        if (exitTime < this.EntryTime)
        {
            throw new InvalidOperationException("La salida no puede ser anterior a la entrada.");
        }

        this.ExitTime = exitTime;
    }
}
