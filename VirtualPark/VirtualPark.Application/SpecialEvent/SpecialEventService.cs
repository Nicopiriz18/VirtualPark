// <copyright file="SpecialEventService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.SpecialEvent;

public class SpecialEventService(ISpecialEventRepository repository, ITicketRepository ticketRepository, IAttractionRepository attractionRepository) : ISpecialEventService
{
    private readonly ISpecialEventRepository repository = repository;
    private readonly ITicketRepository ticketRepository = ticketRepository;
    private readonly IAttractionRepository attractionRepository = attractionRepository;

    public Domain.SpecialEvent Create(string name, DateTime date, int maxCapacity, decimal additionalCost)
    {
        var newEvent = new Domain.SpecialEvent(name, date, maxCapacity, additionalCost);
        return this.repository.Add(newEvent);
    }

    public IEnumerable<Domain.SpecialEvent> GetAll() => this.repository.GetAll();

    public Domain.SpecialEvent? GetById(Guid id) => this.repository.GetById(id);

    public void Delete(Guid id) => this.repository.Delete(id);

    public void AddAttraction(Guid eventId, Guid attractionId)
    {
        var specialEvent = this.repository.GetById(eventId)
                           ?? throw new KeyNotFoundException("Event not found");

        var attraction = this.attractionRepository.GetById(attractionId)
                         ?? throw new KeyNotFoundException("Attraction not found");

        specialEvent.AddAttraction(attraction);
        this.repository.Update(eventId, specialEvent);
    }

    public void RemoveAttraction(Guid eventId, Guid attractionId)
    {
        var specialEvent = this.repository.GetById(eventId)
                           ?? throw new ArgumentException("Event not found");

        specialEvent.RemoveAttraction(attractionId);
        this.repository.Update(eventId, specialEvent);
    }

    public bool HasCapacity(Guid eventId, int ticketsRequested)
    {
        var specialEvent = this.repository.GetById(eventId)
                           ?? throw new ArgumentException("Event not found");

        // Count tickets already sold for this event
        var ticketsSold = this.ticketRepository.CountTicketsByEventId(eventId);

        var remaining = specialEvent.MaxCapacity - ticketsSold;
        return ticketsRequested <= remaining;
    }

    public int GetAvailableCapacity(Guid eventId)
    {
        var specialEvent = this.repository.GetById(eventId)
                           ?? throw new ArgumentException("Event not found");

        var ticketsSold = this.ticketRepository.CountTicketsByEventId(eventId);
        return specialEvent.MaxCapacity - ticketsSold;
    }
}
