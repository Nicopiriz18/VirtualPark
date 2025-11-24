// <copyright file="AttractionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Attractions;

public class AttractionService(IAttractionRepository repository) : IAttractionService
{
    public IEnumerable<Attraction> GetAll() => repository.GetAll();

    public Attraction? GetById(Guid id) => repository.GetById(id);

    public Attraction Create(Attraction attraction) => repository.Add(attraction);

    public void Update(Guid id, Attraction attraction)
    {
        // Get the existing attraction
        var existingAttraction = repository.GetById(id) ?? throw new KeyNotFoundException($"Attraction with id {id} not found");

        // Update its properties using the domain method
        existingAttraction.ModifyAttraction(
            attraction.Name,
            attraction.Description,
            attraction.Type,
            attraction.MinAge,
            attraction.Capacity);

        repository.Update(id, existingAttraction);
    }

    public void Delete(Guid id) => repository.Delete(id);
}
