// <copyright file="AttractionIncidenceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Attractions;

public class AttractionIncidenceService(IAttractionRepository attractionRepository, IIncidenceRepository incidenceRepository) : IAttractionIncidenceService
{
    private readonly IAttractionRepository attractionRepository = attractionRepository;
    private readonly IIncidenceRepository incidenceRepository = incidenceRepository;

    public Incidence AddIncidence(Guid attractionId, string title, string description, bool status, DateTime date)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();

        // Create incidence directly without using the Attraction aggregate
        var incidence = new Incidence(title, description, status, date, attractionId);
        return this.incidenceRepository.Add(incidence);
    }

    public void RemoveIncidence(Guid attractionId, Guid incidenceId)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();

        // Validate incidence exists and belongs to the attraction
        var incidence = this.incidenceRepository.GetById(incidenceId) ?? throw new KeyNotFoundException();
        if (incidence.AttractionId != attractionId)
        {
            throw new InvalidOperationException("La incidencia no pertenece a la atracción especificada.");
        }

        this.incidenceRepository.Delete(incidenceId);
    }

    public void CloseIncidence(Guid attractionId, Guid incidenceId)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();

        // Get and validate incidence
        var incidence = this.incidenceRepository.GetById(incidenceId) ?? throw new KeyNotFoundException();
        if (incidence.AttractionId != attractionId)
        {
            throw new InvalidOperationException("La incidencia no pertenece a la atracción especificada.");
        }

        incidence.Close();
        this.incidenceRepository.Update(incidence);
    }

    public void ReopenIncidence(Guid attractionId, Guid incidenceId)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();

        // Get and validate incidence
        var incidence = this.incidenceRepository.GetById(incidenceId) ?? throw new KeyNotFoundException();
        if (incidence.AttractionId != attractionId)
        {
            throw new InvalidOperationException("La incidencia no pertenece a la atracción especificada.");
        }

        incidence.Reopen();
        this.incidenceRepository.Update(incidence);
    }

    public IEnumerable<Incidence> GetIncidences(Guid attractionId)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();
        return this.incidenceRepository.GetByAttractionId(attractionId);
    }

    public IEnumerable<Incidence> GetActiveIncidences(Guid attractionId)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId) ?? throw new KeyNotFoundException();
        return this.incidenceRepository.GetByAttractionId(attractionId).Where(i => i.Status);
    }
}
