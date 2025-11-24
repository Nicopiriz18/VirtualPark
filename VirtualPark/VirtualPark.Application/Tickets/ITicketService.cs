// <copyright file="ITicketService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Tickets;

public interface ITicketService
{
    public Ticket PurchaseTicket(Guid visitorId, DateTime visitDate, TicketType ticketType);

    public Ticket PurchaseSpecialEventTicket(Guid visitorId, DateTime visitDate, Guid specialEventId);
}
