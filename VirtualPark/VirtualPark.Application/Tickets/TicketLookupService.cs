// <copyright file="TicketLookupService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tickets;

public class TicketLookupService(ITicketRepository ticketRepository, IVisitorRepository visitorRepository) : ITicketLookupService
{
    private readonly ITicketRepository ticketRepository = ticketRepository;
    private readonly IVisitorRepository visitorRepository = visitorRepository;

    public Ticket GetTicketByQrCode(Guid qrCode)
    {
        var ticket = this.ticketRepository.GetByQrCode(qrCode)
            ?? throw new InvalidOperationException($"Ticket with QR code {qrCode} not found");

        // Load the visitor to enable age validation
        var visitor = this.visitorRepository.GetById(ticket.VisitorId)
            ?? throw new InvalidOperationException("Visitor not found");

        ticket.Visitor = visitor;
        return ticket;
    }

    public Ticket GetTicketByVisitorAndDate(Guid visitorId, DateTime visitDate)
    {
        var visitor = this.visitorRepository.GetById(visitorId)
            ?? throw new InvalidOperationException("Visitor not found");

        var tickets = this.ticketRepository.GetAll()
            .Where(t => t.VisitorId == visitorId && t.VisitDate.Date == visitDate.Date)
            .ToList();

        if (!tickets.Any())
        {
            throw new InvalidOperationException($"No valid ticket found for visitor on {visitDate:yyyy-MM-dd}");
        }

        // Use the most recent ticket
        var ticket = tickets.OrderByDescending(t => t.VisitDate).First();
        ticket.Visitor = visitor;

        return ticket;
    }
}
