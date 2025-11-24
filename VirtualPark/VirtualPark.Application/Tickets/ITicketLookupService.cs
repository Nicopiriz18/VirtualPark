// <copyright file="ITicketLookupService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Tickets;

public interface ITicketLookupService
{
    Ticket GetTicketByQrCode(Guid qrCode);

    Ticket GetTicketByVisitorAndDate(Guid visitorId, DateTime visitDate);
}
