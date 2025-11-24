// <copyright file="IAttractionIncidenceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Attractions;

public interface IAttractionIncidenceService
{
    Incidence AddIncidence(Guid attractionId, string title, string description, bool status, DateTime date);

    void RemoveIncidence(Guid attractionId, Guid incidenceId);

    void CloseIncidence(Guid attractionId, Guid incidenceId);

    void ReopenIncidence(Guid attractionId, Guid incidenceId);

    IEnumerable<Incidence> GetIncidences(Guid attractionId);

    IEnumerable<Incidence> GetActiveIncidences(Guid attractionId);
}
