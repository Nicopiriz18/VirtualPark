// <copyright file="IVisitorRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IVisitorRepository
{
    IEnumerable<Visitor> GetAll();

    Visitor? GetById(Guid id);

    Visitor? GetByNfcId(Guid nfcId);

    Visitor Add(Visitor visitor);

    bool Update(Guid id, Visitor visitor);
}
