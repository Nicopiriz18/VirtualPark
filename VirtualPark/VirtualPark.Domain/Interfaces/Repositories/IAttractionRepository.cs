// <copyright file="IAttractionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IAttractionRepository
{
    IEnumerable<Attraction> GetAll();

    Attraction? GetById(Guid id);

    Attraction Add(Attraction attraction);

    void Update(Guid id, Attraction attraction);

    void Delete(Guid id);
}
