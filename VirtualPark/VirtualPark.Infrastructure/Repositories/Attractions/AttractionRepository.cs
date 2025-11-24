// <copyright file="AttractionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class AttractionRepository(ParkDbContext context) : IAttractionRepository
{
    private readonly ParkDbContext context = context;

    public Attraction Add(Attraction attraction)
    {
        this.context.Attractions.Add(attraction);
        this.context.SaveChanges();
        return attraction;
    }

    public IEnumerable<Attraction> GetAll() =>
        this.context.Attractions.ToList();

    public Attraction? GetById(Guid id)
    {
        // Include Incidences collection so it's loaded and tracked
        return this.context.Attractions
            .Include(a => a.Incidences)
            .FirstOrDefault(a => a.Id == id);
    }

    public void Delete(Guid id)
    {
        var entity = this.context.Attractions.Find(id);
        if (entity is not null)
        {
            this.context.Attractions.Remove(entity);
            this.context.SaveChanges();
        }
    }

    public void Update(Guid id, Attraction attraction)
    {
        var existing = this.context.Attractions.Find(id);
        if (existing is null)
        {
            throw new KeyNotFoundException($"Attraction with id {id} not found");
        }

        existing.Name = attraction.Name;
        existing.Description = attraction.Description;
        existing.Type = attraction.Type;
        existing.MinAge = attraction.MinAge;
        existing.Capacity = attraction.Capacity;

        this.context.SaveChanges();
    }
}
