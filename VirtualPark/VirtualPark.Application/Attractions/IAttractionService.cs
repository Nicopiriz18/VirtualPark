// <copyright file="IAttractionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Attractions;

public interface IAttractionService
{
    IEnumerable<Attraction> GetAll();

    Attraction? GetById(Guid id);

    Attraction Create(Attraction attraction);

    void Update(Guid id, Attraction attraction);

    void Delete(Guid id);
}
