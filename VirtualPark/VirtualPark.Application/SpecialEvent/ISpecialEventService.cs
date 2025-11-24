// <copyright file="ISpecialEventService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.SpecialEvent;

public interface ISpecialEventService
{
    Domain.SpecialEvent Create(string name, DateTime date, int maxCapacity, decimal additionalCost);

    IEnumerable<Domain.SpecialEvent> GetAll();

    Domain.SpecialEvent? GetById(Guid id);

    void Delete(Guid id);

    void AddAttraction(Guid eventId, Guid attractionId);

    void RemoveAttraction(Guid eventId, Guid attractionId);

    bool HasCapacity(Guid eventId, int ticketsRequested);

    int GetAvailableCapacity(Guid eventId);
}
