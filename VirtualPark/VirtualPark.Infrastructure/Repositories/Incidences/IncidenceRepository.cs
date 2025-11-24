// <copyright file="IncidenceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class IncidenceRepository(ParkDbContext context) : IIncidenceRepository
{
    private readonly ParkDbContext context = context;

    public Incidence Add(Incidence incidence)
    {
        this.context.Incidences.Add(incidence);
        this.context.SaveChanges();
        return incidence;
    }

    public Incidence? GetById(Guid id)
    {
        return this.context.Incidences.FirstOrDefault(i => i.Id == id);
    }

    public IEnumerable<Incidence> GetByAttractionId(Guid attractionId)
    {
        return this.context.Incidences
            .Where(i => i.AttractionId == attractionId)
            .ToList();
    }

    public void Update(Incidence incidence)
    {
        this.context.Incidences.Update(incidence);
        this.context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var entity = this.context.Incidences.Find(id);
        if (entity is not null)
        {
            this.context.Incidences.Remove(entity);
            this.context.SaveChanges();
        }
    }
}
