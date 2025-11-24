// <copyright file="SpecialEventRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class SpecialEventRepository(ParkDbContext context) : ISpecialEventRepository
{
    private readonly ParkDbContext context = context;

    public IEnumerable<SpecialEvent> GetAll()
    {
        return this.context.SpecialEvents.ToList();
    }

    public SpecialEvent? GetById(Guid id)
    {
        return this.context.SpecialEvents.Find(id);
    }

    public SpecialEvent Add(SpecialEvent specialEvent)
    {
        this.context.SpecialEvents.Add(specialEvent);
        this.context.SaveChanges();
        return specialEvent;
    }

    public void Delete(Guid id)
    {
        var entity = this.context.SpecialEvents.Find(id);
        if (entity is not null)
        {
            this.context.SpecialEvents.Remove(entity);
            this.context.SaveChanges();
        }
    }

    public void Update(Guid id, SpecialEvent specialEvent)
    {
        var existing = this.context.SpecialEvents.Find(id);
        if (existing is not null)
        {
            existing.Name = specialEvent.Name;
            existing.Date = specialEvent.Date;
            existing.AdditionalCost = specialEvent.AdditionalCost;
            existing.MaxCapacity = specialEvent.MaxCapacity;
            this.context.SaveChanges();
        }
    }
}
