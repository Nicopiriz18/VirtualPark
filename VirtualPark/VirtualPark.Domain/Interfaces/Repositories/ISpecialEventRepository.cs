// <copyright file="ISpecialEventRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface ISpecialEventRepository
{
    IEnumerable<SpecialEvent> GetAll();

    SpecialEvent? GetById(Guid id);

    SpecialEvent Add(SpecialEvent specialEvent);

    void Delete(Guid id);

    void Update(Guid id, SpecialEvent specialEvent);
}
