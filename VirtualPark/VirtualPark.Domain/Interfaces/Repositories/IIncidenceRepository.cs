// <copyright file="IIncidenceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IIncidenceRepository
{
    Incidence Add(Incidence incidence);

    Incidence? GetById(Guid id);

    IEnumerable<Incidence> GetByAttractionId(Guid attractionId);

    void Update(Incidence incidence);

    void Delete(Guid id);
}
