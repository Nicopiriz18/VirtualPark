// <copyright file="Ticket.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain;

public class Ticket
{
    public Ticket(DateTime visitDate, TicketType type, Guid qrCode, Guid? specialEventId = null)
    {
        if (qrCode == Guid.Empty)
        {
            throw new ArgumentException("QrCode cannot be empty.", nameof(qrCode));
        }

        this.Id = Guid.NewGuid();
        this.VisitDate = visitDate;
        this.Type = type;
        this.QrCode = qrCode;
        this.SpecialEventId = specialEventId;
    }

    public Guid Id { get; private set; }

    public DateTime VisitDate { get; private set; }

    public TicketType Type { get; private set; }

    public Guid? SpecialEventId { get; set; }

    public Guid QrCode { get; private set; }

    // relación con Visitor
    public Guid VisitorId { get; set; }

    public Visitor Visitor { get; set; } = null!;
}
