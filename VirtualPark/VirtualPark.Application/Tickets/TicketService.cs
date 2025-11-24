// <copyright file="TicketService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Tickets;

public class TicketService(
    ITicketRepository ticketRepository,
    IVisitorRepository visitorRepository,
    ISpecialEventRepository specialEventRepository,
    IMaintenanceRepository maintenanceRepository) : ITicketService
{
    private readonly ITicketRepository ticketRepository = ticketRepository;
    private readonly IVisitorRepository visitorRepository = visitorRepository;
    private readonly ISpecialEventRepository specialEventRepository = specialEventRepository;
    private readonly IMaintenanceRepository maintenanceRepository = maintenanceRepository;

    public Ticket PurchaseSpecialEventTicket(Guid visitorId, DateTime visitDate, Guid specialEventId)
    {
        // Validate visitor exists
        _ = this.GetVisitorOrThrow(visitorId);

        // Validate special event exists
        var specialEvent = this.specialEventRepository.GetById(specialEventId);
        if (specialEvent == null)
        {
            throw new ArgumentException("Special event not found", nameof(specialEventId));
        }

        // Validate capacity: count current tickets sold for this event
        var ticketsSold = this.ticketRepository.CountTicketsByEventId(specialEventId);
        if (ticketsSold >= specialEvent.MaxCapacity)
        {
            throw new InvalidOperationException($"Event has reached maximum capacity ({specialEvent.MaxCapacity} tickets)");
        }

        // Prevent scheduling if any attraction of the event is under maintenance
        foreach (var attraction in specialEvent.Attractions)
        {
            if (this.IsUnderMaintenance(attraction.Id, visitDate))
            {
                throw new InvalidOperationException($"La atracción '{attraction.Name}' tiene mantenimiento programado para la fecha solicitada.");
            }
        }

        // Create ticket with unique QR code
        var qrCode = Guid.NewGuid();
        var ticket = new Ticket(visitDate, TicketType.SpecialEvent, qrCode, specialEventId)
        {
            VisitorId = visitorId,
        };

        return this.ticketRepository.Add(ticket);
    }

    public Ticket PurchaseTicket(Guid visitorId, DateTime visitDate, TicketType ticketType)
    {
        _ = this.GetVisitorOrThrow(visitorId);

        var qrCode = Guid.NewGuid();
        var ticket = new Ticket(visitDate, ticketType, qrCode, null)
        {
            VisitorId = visitorId,
        };

        return this.ticketRepository.Add(ticket);
    }

    private Visitor GetVisitorOrThrow(Guid visitorId)
    {
        return this.visitorRepository.GetById(visitorId)
               ?? throw new ArgumentException("Visitor not found", nameof(visitorId));
    }

    private bool IsUnderMaintenance(Guid attractionId, DateTime referenceTime)
    {
        var maintenances = this.maintenanceRepository.GetByAttractionId(attractionId);
        foreach (var maintenance in maintenances)
        {
            var start = maintenance.ScheduledDate.Date.Add(maintenance.StartTime);
            var end = start + maintenance.EstimatedDuration;
            if (referenceTime >= start && referenceTime <= end)
            {
                return true;
            }
        }

        return false;
    }
}
